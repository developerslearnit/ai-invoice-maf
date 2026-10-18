using InvoiceApp.Web.Models.ViewModels;
using InvoiceApp.Web.Services.ApiClients;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Web.Controllers;

[Route("")]
[Route("dashboard")]
public sealed class DashboardController : Controller
{
    private readonly IDashboardApiClient _dashboardApiClient;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardApiClient dashboardApiClient, ILogger<DashboardController> logger)
    {
        _dashboardApiClient = dashboardApiClient;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";

        // Fetch dashboard data in parallel
        var summaryTask = _dashboardApiClient.GetSummaryAsync(cancellationToken);
        var recentTask = _dashboardApiClient.GetRecentInvoicesAsync(6, cancellationToken);
        var overdueTask = _dashboardApiClient.GetOverdueInvoicesAsync(cancellationToken);

        await Task.WhenAll(summaryTask, recentTask, overdueTask);

        var summaryResult = await summaryTask;
        var recentResult = await recentTask;
        var overdueResult = await overdueTask;

        string? errorMessage = null;
        if (!summaryResult.IsSuccess || !recentResult.IsSuccess)
        {
            errorMessage = summaryResult.ErrorMessage ?? recentResult.ErrorMessage ?? "Could not load complete dashboard data.";
            _logger.LogWarning("Dashboard partial data load failure: {Error}", errorMessage);
        }

        var viewModel = new DashboardViewModel
        {
            Summary = summaryResult.Data ?? new(0, 0, 0, 0, 0, 0, 0, 0),
            RecentInvoices = recentResult.Data ?? [],
            OverdueInvoices = overdueResult.Data ?? [],
            ErrorMessage = errorMessage
        };

        return View(viewModel);
    }

    [HttpGet("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
