using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.Application.Features.Users.Queries.GetUserStats;

/// <summary>
/// Handler for GetUserStatsQuery
/// </summary>
public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, GetUserStatsQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserStatsQueryHandler> _logger;

    public GetUserStatsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserStatsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetUserStatsQueryResponse> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting user statistics for user ID: {UserId}", request.UserId);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {request.UserId} not found.");
            }

            // Get user transactions
            var transactions = await _unitOfWork.Transactions.GetAsync(
                filter: t => t.UserId == request.UserId,
                cancellationToken: cancellationToken);

            // Calculate statistics
            var transactionsList = transactions.ToList();
            var totalTransactions = transactionsList.Count;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var transactionsLast30Days = transactionsList.Where(t => t.CreatedAt >= thirtyDaysAgo).Count();
            var totalValue = transactionsList.Where(t => t.TotalValue.HasValue).Sum(t => t.TotalValue!.Value);

            var accountAgeInDays = (DateTime.UtcNow - user.CreatedAt).Days;
            var daysSinceLastLogin = user.LastLoginAt.HasValue 
                ? (DateTime.UtcNow - user.LastLoginAt.Value).Days 
                : (int?)null;

            // Calculate login frequency (assuming we track login history - placeholder calculation)
            var loginFrequency = accountAgeInDays > 0 ? (double)(user.LastLoginAt.HasValue ? 30 : 0) / accountAgeInDays * 30 : 0;

            var response = new GetUserStatsQueryResponse
            {
                TotalTransactions = totalTransactions,
                TransactionsLast30Days = transactionsLast30Days,
                TotalTransactionValue = totalValue,
                DaysSinceLastLogin = daysSinceLastLogin,
                AccountAgeInDays = accountAgeInDays,
                LoginFrequency = loginFrequency
            };

            _logger.LogInformation("Successfully calculated user statistics for user ID: {UserId}", request.UserId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics for user ID: {UserId}", request.UserId);
            throw;
        }
    }
}
