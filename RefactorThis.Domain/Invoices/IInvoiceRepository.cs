namespace RefactorThis.Domain.Invoices
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);

        void SaveInvoice(Invoice invoice);

        void Add(Invoice invoice);
    }
}
