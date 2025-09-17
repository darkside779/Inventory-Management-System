using MediatR;

namespace InventoryManagement.Application.Features.Users.Queries.GetUserStats;

/// <summary>
/// Query to get user statistics
/// </summary>
public class GetUserStatsQuery : IRequest<GetUserStatsQueryResponse>
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    public GetUserStatsQuery(int userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// Response for GetUserStatsQuery
/// </summary>
public class GetUserStatsQueryResponse
{
    /// <summary>
    /// Total transactions created by user
    /// </summary>
    public int TotalTransactions { get; set; }

    /// <summary>
    /// Transactions in last 30 days
    /// </summary>
    public int TransactionsLast30Days { get; set; }

    /// <summary>
    /// Total value of transactions
    /// </summary>
    public decimal TotalTransactionValue { get; set; }

    /// <summary>
    /// Days since last login
    /// </summary>
    public int? DaysSinceLastLogin { get; set; }

    /// <summary>
    /// Account age in days
    /// </summary>
    public int AccountAgeInDays { get; set; }

    /// <summary>
    /// Login frequency (logins per month)
    /// </summary>
    public double LoginFrequency { get; set; }
}
