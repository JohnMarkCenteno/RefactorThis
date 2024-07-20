using RefactorThis.Persistence;

namespace RefactorThis.Domain.Strategies.Payment
{
    public interface IInvoicePaymentStrategy
    {
        string ProcessPayment(Invoice invoice, Persistence.Payment payment);
    }
}
