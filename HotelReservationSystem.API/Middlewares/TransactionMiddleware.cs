using Infrastructure.Data;

namespace HotelReservationSystem.API.Middlewares
{
    public class TransactionMiddleware : IMiddleware
    {
        private readonly Context _context;

        public TransactionMiddleware(Context context)
        {
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // apply transactions to methods that modify data
            var httpMethod = context.Request.Method;
            if (httpMethod == HttpMethods.Get || httpMethod == HttpMethods.Head || httpMethod == HttpMethods.Options)
            {
                await next(context);
                return;
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await next(context);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                // i used throw to save the stack trace
                throw;
            }
        }
    }
}
