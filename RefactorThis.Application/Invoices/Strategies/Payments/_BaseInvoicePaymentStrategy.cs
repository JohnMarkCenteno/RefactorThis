using RefactorThis.Domain.Invoices;
using RefactorThis.Domain.Payments;
using System;
using System.Linq;

namespace RefactorThis.Application.Invoices.Strategies.Payments
{
    public abstract class BaseInvoicePaymentStrategy : IInvoicePaymentStrategy
    {
        public string ProcessPayment(Invoice inv, Payment payment)
        {
            if (inv.Amount == 0)
            {
                return inv.Payments == null || !inv.Payments.Any()
                    ? PaymentMessages.NoPaymentNeeded
                    : throw new InvalidOperationException(PaymentMessages.InvalidInvoiceState);
            }

            if (inv.Payments != null && inv.Payments.Any())
            {
                if (inv.Payments.Sum(x => x.Amount) != 0 && inv.Amount == inv.Payments.Sum(x => x.Amount))
                {
                    return PaymentMessages.InvoiceAlreadyFullyPaid;
                }

                if (payment.Amount > (inv.Amount - inv.AmountPaid))
                {
                    return PaymentMessages.PaymentGreaterThanPartialAmountRemaining;
                }

                HandlePartialPayment(inv, payment);

                return (inv.Amount - inv.AmountPaid) == 0
                    ? PaymentMessages.FinalPartialPaymentReceivedFullyPaid
                    : PaymentMessages.AnotherPartialPaymentReceivedStillNotFullyPaid;
            }

            if (inv.Amount == payment.Amount)
            {
                HandleInitialPayment(inv, payment);
                return PaymentMessages.InvoiceIsNowFullyPaid;
            }
            else if (payment.Amount > inv.Amount)
            {
                return PaymentMessages.PaymentGreaterThanInvoiceAmount;
            }
            else
            {
                HandleInitialPayment(inv, payment);
                return PaymentMessages.InvoiceIsNowPartiallyPaid;
            }
        }

        protected abstract void HandlePartialPayment(Invoice inv, Payment payment);
        protected abstract void HandleInitialPayment(Invoice inv, Payment payment);
    }
}
