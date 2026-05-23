using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.User;
using Application.Helper;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Auth.Command
{
    public sealed record RefreshTokenCommand(RefreshTokenDto Model) : IRequest<ResponseViewModel<LoginResponseDto>>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseViewModel<LoginResponseDto>>
    {
        private readonly IRepository<Domain.Entities.User.User> _userRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public RefreshTokenCommandHandler(IRepository<Domain.Entities.User.User> userRepository,ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ResponseViewModel<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // 1. Extract the UserId from the expired Access Token
            var principal = _tokenGenerator.GetPrincipalFromExpiredToken(request.Model.Token);
            var userIdClaim = principal.FindFirst("UserID")?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return ResponseViewModel<LoginResponseDto>.Failure(Enum.ErrorCode.InvalidToken, "Invalid Token!");

            var user = await _userRepository.GetByIDAsync(userId, cancellationToken);

            if (user is null || user.RefreshToken != request.Model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ResponseViewModel<LoginResponseDto>.Failure(Enum.ErrorCode.UnExpectedError, "Invalid Request Please Login again!");
            }

            // 3. If it passes the check above (meaning the refresh token is valid and matches the database)
            // Generate a new access token and refresh token
            var newToken = _tokenGenerator.Generate(user.ID, user.Name, user.Role.ToString());
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            // 4. Update the database with the new refresh token
            user.RefreshToken = newRefreshToken;
            await _userRepository.SaveChangesAsync(cancellationToken);

            // 5. Return the data
            var responseDto = new LoginResponseDto(newToken, newRefreshToken, user.Name, user.Role.ToString());

            return ResponseViewModel<LoginResponseDto>.Success(responseDto, "Token and Refresh Token Generated Successfully");
        }
    }
}
