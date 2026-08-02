namespace Vuzol.ViewModel.Model
{
    public class Property
    {
        public string FactoryNumber { get; set; }
        public string Name { get; set; }
        public string InventoryNumber { get; set; }
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BookId { get; set; }
        public int OrderBookId { get; set; }
        public int FormId { get; set; }
        public int CategoryId { get; set; }
        public int MaterialResourcesId { get; set; }
        public int QuantityTypeId { get; set; }
        public string? FormName { get; set; }
        public DateTime FormDate { get; set; }
        public int OrderId { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime DLM { get; set; }
        public int FIO_R { get; set; }
        public int FIO_V { get; set; }
        public string FIO_R_STR { get; set; }
        public string FIO_V_STR { get; set; }
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
        public decimal Cost { get; set; }
        public Property(string factoryNumber, string? name,
            string inventoryNumber, int invoiceId,
            int bookId, int orderBookId, int formId,
            string? formName, DateTime formDate,
            int orderId, int propertyTypeId,
            string additionalnfo,
            DateTime dLM, int fio_r, int fio_v, string fio_r_str, string fio_v_str, string unit_name,
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
            AdditionalInfo = additionalnfo;
            DLM = dLM;
            FIO_R = fio_r;
            FIO_V = fio_v;
            FIO_R_STR = fio_r_str;
            FIO_V_STR = fio_v_str;
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
