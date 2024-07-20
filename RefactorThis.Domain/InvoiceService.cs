using RefactorThis.Domain.Messages;
using RefactorThis.Domain.Strategies.Payment;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain
{
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;
        private readonly Dictionary<InvoiceType, IInvoicePaymentStrategy> _paymentStrategies;

        public InvoiceService(InvoiceRepository invoiceRepository, Dictionary<InvoiceType, IInvoicePaymentStrategy> paymentStrategies)
        {
            _invoiceRepository = invoiceRepository;
            _paymentStrategies = paymentStrategies;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            if (inv == null)
            {
                throw new InvalidOperationException(InvoicePaymentMessages.NoMatchingInvoiceForPayment);
            }

            if (!_paymentStrategies.TryGetValue(inv.Type, out var strategy))
            {
                throw new ArgumentOutOfRangeException(InvoicePaymentMessages.UnsupportedInvoiceType);
            }

            var responseMessage = strategy.ProcessPayment(inv, payment);
            _invoiceRepository.SaveInvoice(inv);

            return responseMessage;
        }
    }
}