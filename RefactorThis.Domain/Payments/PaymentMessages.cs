namespace RefactorThis.Domain.Payments
{
    public static class PaymentMessages
    {
        public const string NoPaymentNeeded = "No payment needed.";
        public const string InvoiceAlreadyFullyPaid = "Invoice was already fully paid.";
        public const string PaymentGreaterThanPartialAmountRemaining = "The payment is greater than the partial amount remaining.";
        public const string PaymentGreaterThanInvoiceAmount = "The payment is greater than the invoice amount.";
        public const string FinalPartialPaymentReceivedFullyPaid = "Final partial payment received, invoice is now fully paid.";
        public const string AnotherPartialPaymentReceivedStillNotFullyPaid = "Another partial payment received, still not fully paid.";
        public const string InvoiceIsNowFullyPaid = "Invoice is now fully paid.";
        public const string InvoiceIsNowPartiallyPaid = "Invoice is now partially paid.";
        public const string InvalidInvoiceState = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
        public const string NoMatchingInvoiceForPayment = "There is no invoice matching this payment.";
        public const string UnsupportedInvoiceType = "Unsupported invoice type.";
    }
}
