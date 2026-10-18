namespace InvoiceApp.Domain.Services;

public interface IInvoiceNumberGenerator
{
    Task<string> NextInvoiceNumberAsync(CancellationToken cancellationToken = default);
}
