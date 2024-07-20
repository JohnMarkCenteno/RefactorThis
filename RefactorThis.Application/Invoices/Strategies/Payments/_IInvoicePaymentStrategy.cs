using RefactorThis.Domain.Invoices;
using RefactorThis.Domain.Payments;

namespace RefactorThis.Application.Invoices.Strategies.Payments
{
    public interface IInvoicePaymentStrategy
    {
        string ProcessPayment(Invoice invoice, Payment payment);
    }
}
