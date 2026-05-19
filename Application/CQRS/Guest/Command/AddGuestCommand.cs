using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Guest;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Application.CQRS.Guest.Command
{
    public sealed record AddGuestCommand(AddGuestDto model) : IRequest<ResponseViewModel<AddGuestDto>>;

    public class AddGuestCommandHandler : IRequestHandler<AddGuestCommand, ResponseViewModel<AddGuestDto>>
    {
        private readonly IRepository<Domain.Entities.Guest.Guest> _Repository;
        public AddGuestCommandHandler(IRepository<Domain.Entities.Guest.Guest> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<AddGuestDto>> Handle(AddGuestCommand request, CancellationToken cancellationToken)
        {

            var GuestEntity = request.model.Map<Domain.Entities.Guest.Guest>();

            await _Repository.AddAsync(GuestEntity,cancellationToken);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if(!IsSaved)
                return ResponseViewModel<AddGuestDto>.Failure(Enum.ErrorCode.AddGuestFail, message: "Add guest fail");

            return ResponseViewModel<AddGuestDto>.Success(GuestEntity.Map<AddGuestDto>(), message: "Guest add successfully");
        }
    }
}
