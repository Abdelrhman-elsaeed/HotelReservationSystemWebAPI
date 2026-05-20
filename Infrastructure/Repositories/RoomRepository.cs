using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RoomRepository : GenericRepository<Room> , IRoomRepository
    {
        public RoomRepository(Context Context) : base(Context)
        {
        }

        public async Task<IEnumerable<Room>> GetRoomsByPredicateAsync(Expression<Func<Room, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var query = this.GetAll();
            if (predicate is not null)
            {
                 query = query.Where(predicate);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<decimal?> GetRoomTotalPriceAsync(int RoomId,CancellationToken cancellationToken)
        {
            // to check offer is available or not
            var currentDate = DateTime.UtcNow; 

            var roomPricingData = await _dbSet
                .Where(r => r.ID == RoomId && !r.Deleted)
                .Select(r => new
                {
                    BasePrice = r.RoomType.Price,
                    FacilitiesTotalPrice = r.RoomFacilities.Sum(rf => rf.Facility.Price),
                    MaxActiveDiscount = r.RoomOffers
                        .Where(ro => ro.Offer.StartDate <= currentDate && ro.Offer.EndDate >= currentDate)
                        .Select(ro => ro.Offer.DiscountPercentage)
                        .OrderByDescending(d => d)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (roomPricingData == null)
            {
                return null;
            }

            // Calculation Logic
            var totalPriceBeforeDiscount = roomPricingData.BasePrice + roomPricingData.FacilitiesTotalPrice;
            var discountAmount = totalPriceBeforeDiscount * (roomPricingData.MaxActiveDiscount / 100m);
            var finalPrice = totalPriceBeforeDiscount - discountAmount;

            return finalPrice;
        }
    }
}
