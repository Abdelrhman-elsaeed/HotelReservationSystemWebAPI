using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.User;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Application.CQRS.Auth.Command
{
    public sealed record RegisterCommand(RegisterDto model,CancellationToken CancellationToken) : IRequest<ResponseViewModel<RegisterResponseDto>>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseViewModel<RegisterResponseDto>>
    {

        private readonly IRepository<Domain.Entities.User.User> _Repository;
        public RegisterCommandHandler(IRepository<Domain.Entities.User.User> Repository)
        {
            _Repository = Repository;   
        }

        public async Task<ResponseViewModel<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var IsUserExistBefore = await _Repository.CheckExistsByConditionAsync(u => u.Username == request.model.Username || u.Email==request.model.Email && !u.Deleted,cancellationToken);
            if (IsUserExistBefore)
                return ResponseViewModel<RegisterResponseDto>.Failure(Enum.ErrorCode.UserExistBefore, message: "This user or email exist before try another one");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.model.Password);

            var RegisterEntity = request.model.Map<Domain.Entities.User.User>();
            RegisterEntity.PasswordHash = hashedPassword;
            RegisterEntity.Role = Domain.Enum.Role.Customer;

            var AddedEntity = await _Repository.AddAsync(RegisterEntity);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<RegisterResponseDto>.Failure(Enum.ErrorCode.RegisterFail, message: "Fail to register user");


            return ResponseViewModel<RegisterResponseDto>.Success(new RegisterResponseDto
                (AddedEntity.ID,
                AddedEntity.Name,
                AddedEntity.Username,
                AddedEntity.Email,
                AddedEntity.Role), message: "User registered successfully!");



        }
    }
}
