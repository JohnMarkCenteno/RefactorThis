using RefactorThis.Domain.Payments;

namespace RefactorThis.Domain.Invoices
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment);
    }
}
