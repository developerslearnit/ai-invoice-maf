using InvoiceApp.Web.Models.Api;

namespace InvoiceApp.Web.Models.ViewModels;

public sealed class DashboardViewModel
{
    public InvoiceSummaryApiModel Summary { get; init; } = new(0, 0, 0, 0, 0, 0, 0, 0);
    public IReadOnlyList<InvoiceListItemApiModel> RecentInvoices { get; init; } = [];
    public IReadOnlyList<InvoiceListItemApiModel> OverdueInvoices { get; init; } = [];
    public string? ErrorMessage { get; init; }

    public double PaidPercentage => Summary.TotalInvoices > 0
        ? Math.Round((double)Summary.PaidInvoices / Summary.TotalInvoices * 100, 1)
        : 0;

    public double SentPercentage => Summary.TotalInvoices > 0
        ? Math.Round((double)Summary.SentInvoices / Summary.TotalInvoices * 100, 1)
        : 0;

    public double OverduePercentage => Summary.TotalInvoices > 0
        ? Math.Round((double)Summary.OverdueInvoices / Summary.TotalInvoices * 100, 1)
        : 0;

    public double DraftPercentage => Summary.TotalInvoices > 0
        ? Math.Round((double)Summary.DraftInvoices / Summary.TotalInvoices * 100, 1)
        : 0;
}
