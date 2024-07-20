using NUnit.Framework;
using RefactorThis.Domain.Messages;
using RefactorThis.Domain.Strategies.Payment;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private Dictionary<InvoiceType, IInvoicePaymentStrategy> _paymentStrategies;
        private InvoiceRepository _repo;

        [SetUp]
        public void SetUp()
        {
            _paymentStrategies = new Dictionary<InvoiceType, IInvoicePaymentStrategy>
            {
                { InvoiceType.Standard, new StandardInvoicePaymentStrategy() },
                { InvoiceType.Commercial, new CommercialInvoicePaymentStrategy() }
            };

            _repo = new InvoiceRepository();
        }

        private Invoice CreateInvoice(decimal amount, decimal amountPaid, List<Payment> payments = null)
        {
            var invoice = new Invoice(_repo)
            {
                Amount = amount,
                AmountPaid = amountPaid,
                Payments = payments ?? new List<Payment>()
            };
            _repo.Add(invoice);
            return invoice;
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment();
            var failureMessage = "";

            try
            {
                var result = paymentProcessor.ProcessPayment(payment);
            }
            catch (InvalidOperationException e)
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual(InvoicePaymentMessages.NoMatchingInvoiceForPayment, failureMessage);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            CreateInvoice(0, 0);
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment();
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.NoPaymentNeeded, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            CreateInvoice(10, 10, new List<Payment> { new Payment { Amount = 10 } });
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment();
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.InvoiceAlreadyFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            CreateInvoice(10, 5, new List<Payment> { new Payment { Amount = 5 } });
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 6 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.PaymentGreaterThanPartialAmountRemaining, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            CreateInvoice(5, 0);
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 6 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.PaymentGreaterThanInvoiceAmount, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            CreateInvoice(10, 5, new List<Payment> { new Payment { Amount = 5 } });
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 5 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.FinalPartialPaymentReceivedFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            CreateInvoice(10, 0, new List<Payment> { new Payment { Amount = 10 } });
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 10 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.InvoiceAlreadyFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            CreateInvoice(10, 5, new List<Payment> { new Payment { Amount = 5 } });
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 1 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.AnotherPartialPaymentReceivedStillNotFullyPaid, result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            CreateInvoice(10, 0);
            var paymentProcessor = new InvoiceService(_repo, _paymentStrategies);
            var payment = new Payment { Amount = 1 };
            var result = paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual(InvoicePaymentMessages.InvoiceIsNowPartiallyPaid, result);
        }
    }
}
