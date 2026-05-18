using Application.DTOS;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Guest.Queries
{
    public sealed record IsGuestExistQuery(int GuestId) : IRequest<ResponseViewModel<bool>>;
    
    public class IsGuestExistQueryHandler : IRequestHandler<IsGuestExistQuery, ResponseViewModel<bool>>
    {

        IRepository<Domain.Entities.Guest.Guest> _Repository;
        public IsGuestExistQueryHandler(IRepository<Domain.Entities.Guest.Guest> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<bool>> Handle(IsGuestExistQuery request, CancellationToken cancellationToken)
        {
            var IsExist = await _Repository.CheckExistsByIDAsync(request.GuestId, cancellationToken);

            if (IsExist)
                return ResponseViewModel<bool>.Success(IsExist, message: "Guest is exist");
            else
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.GuestNotFound, "Guest not found on the system");
        }
    }
}
