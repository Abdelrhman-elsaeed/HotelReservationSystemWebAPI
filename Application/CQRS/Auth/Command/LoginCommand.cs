using Application.DTOS;
using Application.DTOS.User;
using Application.Helper;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Auth.Command
{
    public sealed record LoginCommand(LoginRequestDto Model) : IRequest<ResponseViewModel<LoginResponseDto>>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseViewModel<LoginResponseDto>>
    {
        private readonly IRepository<Domain.Entities.User.User> _userRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginCommandHandler(IRepository<Domain.Entities.User.User> userRepository,ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<ResponseViewModel<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            
            // Validate User exist
            var user = await _userRepository.GetByConditionAsync(u => u.Username == request.Model.Username, cancellationToken);

            if (user is null)
                return ResponseViewModel<LoginResponseDto>.Failure(Enum.ErrorCode.UserNotFound, message: "User not found!");

            // 2. Validate Password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Model.Password, user.PasswordHash);

            if (!isPasswordValid)
                return ResponseViewModel<LoginResponseDto>.Failure(Enum.ErrorCode.InvalidPassword, message: "Incorrect password");

            // 3. Generate Token
            var token = _tokenGenerator.Generate(user.ID, user.Name, user.Role.ToString());

            // 4. Return Result
            var responseDto = new LoginResponseDto(token, user.Name, user.Role.ToString());

            return ResponseViewModel<LoginResponseDto>.Success(responseDto, message: "Logged In successfully");
        }
    }
}
