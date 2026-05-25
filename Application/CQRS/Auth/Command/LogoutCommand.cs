using Application.DTOS;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Auth.Command
{
    public sealed record LogoutCommand(int UserId,CancellationToken CancellationToken) : IRequest<ResponseViewModel<bool>>;
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.User.User> _userRepository;

        public LogoutCommandHandler(IRepository<Domain.Entities.User.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseViewModel<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIDAsync(request.UserId, cancellationToken);

            if (user is null)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.UserNotFound, "User Not Found!");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _userRepository.UpdateInclude(user, nameof(Domain.Entities.User.User.RefreshToken), nameof(Domain.Entities.User.User.RefreshTokenExpiryTime));

            var IsSaved = await _userRepository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.LogoutFail, message: "Fail to logout!");

            return ResponseViewModel<bool>.Success(IsSaved, "Logout Successfully");
        }
    }
}
