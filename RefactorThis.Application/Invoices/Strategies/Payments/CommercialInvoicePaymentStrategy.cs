using RefactorThis.Domain.Invoices;
using RefactorThis.Domain.Payments;

namespace RefactorThis.Application.Invoices.Strategies.Payments
{
    public class CommercialInvoicePaymentStrategy : BaseInvoicePaymentStrategy
    {
        protected override void HandlePartialPayment(Invoice inv, Payment payment)
        {
            inv.AmountPaid += payment.Amount;
            inv.TaxAmount += payment.Amount * 0.14m;
            inv.Payments.Add(payment);
        }

        protected override void HandleInitialPayment(Invoice inv, Payment payment)
        {
            inv.AmountPaid = payment.Amount;
            inv.TaxAmount = payment.Amount * 0.14m;
            inv.Payments.Add(payment);
        }
    }
}
