using System.Globalization;

namespace InvoiceApp.Web.Models.ViewModels;

public static class CurrencyHelper
{
    private static readonly CultureInfo NairaCulture = new("en-NG")
    {
        NumberFormat =
        {
            CurrencySymbol = "₦",
            CurrencyPositivePattern = 0, // ₦1,000.00
            CurrencyNegativePattern = 1  // -₦1,000.00
        }
    };

    public static string FormatNaira(decimal amount)
    {
        return $"₦{amount:N2}";
    }

    public static string FormatCompact(decimal amount)
    {
        if (Math.Abs(amount) >= 1_000_000)
            return $"₦{(amount / 1_000_000m):0.#}M";
        if (Math.Abs(amount) >= 1_000)
            return $"₦{(amount / 1_000m):0.#}K";
        return $"₦{amount:N0}";
    }
}
