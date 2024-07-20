using RefactorThis.Persistence;

namespace RefactorThis.Domain.Strategies.Payment
{
    public class StandardInvoicePaymentStrategy : BaseInvoicePaymentStrategy
    {
        protected override void HandlePartialPayment(Invoice inv, Persistence.Payment payment)
        {
            inv.AmountPaid += payment.Amount;
            inv.Payments.Add(payment);
        }

        protected override void HandleInitialPayment(Invoice inv, Persistence.Payment payment)
        {
            inv.AmountPaid = payment.Amount;
            inv.TaxAmount = payment.Amount * 0.14m;
            inv.Payments.Add(payment);
        }
    }
}
