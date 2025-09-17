using MediatR;
using InventoryManagement.Application.DTOs;

namespace InventoryManagement.Application.Features.Dashboard.Queries.GetDashboardData;

/// <summary>
/// Query to get comprehensive dashboard data including KPIs, recent activities, and alerts
/// </summary>
public class GetDashboardDataQuery : IRequest<DashboardDataDto>
{
    public string UserId { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
