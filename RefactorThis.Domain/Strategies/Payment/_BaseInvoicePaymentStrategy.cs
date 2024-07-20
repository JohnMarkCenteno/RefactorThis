using RefactorThis.Domain.Messages;
using RefactorThis.Persistence;
using System;
using System.Linq;

namespace RefactorThis.Domain.Strategies.Payment
{
    public abstract class BaseInvoicePaymentStrategy : IInvoicePaymentStrategy
    {
        public string ProcessPayment(Invoice inv, Persistence.Payment payment)
        {
            if (inv.Amount == 0)
            {
                return inv.Payments == null || !inv.Payments.Any()
                    ? InvoicePaymentMessages.NoPaymentNeeded
                    : throw new InvalidOperationException(InvoicePaymentMessages.InvalidInvoiceState);
            }

            if (inv.Payments != null && inv.Payments.Any())
            {
                if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                {
                    return InvoicePaymentMessages.InvoiceAlreadyFullyPaid;
                }

                if (payment.Amount > (inv.Amount - inv.AmountPaid))
                {
                    return InvoicePaymentMessages.PaymentGreaterThanPartialAmountRemaining;
                }

                HandlePartialPayment(inv, payment);

                return (inv.Amount - inv.AmountPaid) == 0
                    ? InvoicePaymentMessages.FinalPartialPaymentReceivedFullyPaid
                    : InvoicePaymentMessages.AnotherPartialPaymentReceivedStillNotFullyPaid;
            }

            if (inv.Amount == payment.Amount)
            {
                HandleInitialPayment(inv, payment);
                return InvoicePaymentMessages.InvoiceIsNowFullyPaid;
            }
            else if (payment.Amount > inv.Amount)
            {
                return InvoicePaymentMessages.PaymentGreaterThanInvoiceAmount;
            }
            else
            {
                HandleInitialPayment(inv, payment);
                return InvoicePaymentMessages.InvoiceIsNowPartiallyPaid;
            }
        }

        protected abstract void HandlePartialPayment(Invoice inv, Persistence.Payment payment);
        protected abstract void HandleInitialPayment(Invoice inv, Persistence.Payment payment);
    }
}
