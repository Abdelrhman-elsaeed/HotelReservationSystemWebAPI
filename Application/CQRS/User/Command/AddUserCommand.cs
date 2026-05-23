using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.User;
using Domain.Repositories.Interfaces;
using MediatR;
namespace Application.CQRS.User.Command
{
    public sealed record AddUserCommand(AddUserDto model) : IRequest<ResponseViewModel<AddUserResponseDto>>;

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ResponseViewModel<AddUserResponseDto>>
    {
        private readonly IRepository<Domain.Entities.User.User> _Repository;
        public AddUserCommandHandler(IRepository<Domain.Entities.User.User> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<AddUserResponseDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var UserEntity = request.model.Map<Domain.Entities.User.User>();

            UserEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.model.Password);

            var AddedEntity = await _Repository.AddAsync(UserEntity, cancellationToken);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<AddUserResponseDto>.Failure(Enum.ErrorCode.AddUserFail, message: "Fail to add user");
            else
                return ResponseViewModel<AddUserResponseDto>.Success(new AddUserResponseDto 
                { ID = AddedEntity.ID
                ,Name=AddedEntity.Name
                ,Email=AddedEntity.Email
                ,Role=AddedEntity.Role}
                , message: "User add successfully");
        }

       
    }
}
