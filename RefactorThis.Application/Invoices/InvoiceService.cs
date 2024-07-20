using RefactorThis.Application.Invoices.Strategies.Payments;
using RefactorThis.Domain.Invoices;
using RefactorThis.Domain.Payments;
using RefactorThis.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace RefactorThis.Application.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly Dictionary<InvoiceType, IInvoicePaymentStrategy> _paymentStrategies;

        public InvoiceService(IInvoiceRepository invoiceRepository, Dictionary<InvoiceType, IInvoicePaymentStrategy> paymentStrategies)
        {
            _invoiceRepository = invoiceRepository;
            _paymentStrategies = paymentStrategies;
        }

        public string ProcessPayment(Payment payment)
        {
            var inv = _invoiceRepository.GetInvoice(payment.Reference);

            if (inv == null)
            {
                throw new InvalidOperationException(PaymentMessages.NoMatchingInvoiceForPayment);
            }

            if (!_paymentStrategies.TryGetValue(inv.Type, out var strategy))
            {
                throw new ArgumentOutOfRangeException(PaymentMessages.UnsupportedInvoiceType);
            }

            var responseMessage = strategy.ProcessPayment(inv, payment);
            _invoiceRepository.SaveInvoice(inv);

            return responseMessage;
        }
    }
}