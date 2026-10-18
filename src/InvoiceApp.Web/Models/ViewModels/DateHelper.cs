namespace InvoiceApp.Web.Models.ViewModels;

public static class DateHelper
{
    public static string Format(DateOnly date) => date.ToString("dd MMM yyyy");
    public static string Format(DateOnly? date) => date.HasValue ? date.Value.ToString("dd MMM yyyy") : "-";
    public static string Format(DateTime dateTime) => dateTime.ToString("dd MMM yyyy, HH:mm");
    public static string Format(DateTime? dateTime) => dateTime.HasValue ? dateTime.Value.ToString("dd MMM yyyy, HH:mm") : "-";
}
