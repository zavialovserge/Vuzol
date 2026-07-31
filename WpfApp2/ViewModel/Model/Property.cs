namespace Vuzol.ViewModel.Model
{
    public class Property
    {
        public int FactoryNumber { get; set; }
        public string Name { get; set; }
        public int InventoryNumber { get; set; }
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BookId { get; set; }
        public int OrderBookId { get; set; }
        public string FormId { get; set; }
        public string? FormName { get; set; }
        public DateTime FormDate { get; set; }
        public int OrderId { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
        public string Additionalnfo { get; set; }
        public DateTime DLM { get; set; }
        public int FIO_R { get; set; }
        public int FIO_I { get; set; }
        public string FIO_R_STR { get; set; }
        public string FIO_I_STR { get; set; }
        public string UnitName { get; set; }
        public int BookPage { get; set; }
        public int OrderBookPage { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string CategoryDescription { get; set; }
        public string MaterialResourcesDescription { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityTypeDescription { get; set; }
        public decimal Price { get; set; }
        public Property(int factoryNumber, string? name,
            int inventoryNumber, int invoiceId,
            int bookId, int orderBookId, string? formId,
            string? formName, DateTime formDate,
            int orderId, int propertyTypeId,
            string? additionalnfo,
            DateTime dLM, int fio_r, int fio_i, string fio_r_str, string fio_i_str, string unit_name,
            int bookPage, int orderBookPage, DateTime orderDate, int status, string categoryDescription, string materialResourcesDescription, string quantityTypeDescription    )
        {
            FactoryNumber = factoryNumber;
            Name = name;
            InventoryNumber = inventoryNumber;
            InvoiceId = invoiceId;
            BookId = bookId;
            OrderBookId = orderBookId;
            FormId = formId;
            FormName = formName;
            FormDate = formDate;
            OrderId = orderId;
            PropertyTypeId = propertyTypeId;
            Additionalnfo = additionalnfo;
            DLM = dLM;
            FIO_R = fio_r;
            FIO_I = fio_i;
            FIO_R_STR = fio_r_str;
            FIO_I_STR = fio_i_str;
            UnitName = unit_name;
            BookPage = bookPage;
            OrderBookPage = orderBookPage;
            OrderDate = orderDate;
            Status = status;
            CategoryDescription = categoryDescription;
            MaterialResourcesDescription = materialResourcesDescription;
            QuantityTypeDescription = quantityTypeDescription;
        }
        public Property()
        {

        }
    }
}
