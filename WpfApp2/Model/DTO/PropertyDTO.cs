namespace Vuzol.Model.Db
{
    public class PropertyDTO
    {
        public string FactoryNumber { get; set; }
        public string? Name { get; set; }
        public string InventoryNumber { get; set; }
        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int BookId { get; set; }
        public int BookPage { get; set; }
        public int OrderBookId { get; set; }
        public int OrderBookPage { get; set; }
        public int FormId { get; set; }
        public string? FormName { get; set; }
        public DateTime FormDate { get; set; }
        public int OrderId { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime DLM { get; set; }
        public int Fio_R { get; set; }
        public int FIO_V { get; set; }
        public string FIO_R_STR { get; set; }
        public string FIO_V_STR { get; set; }
        public string UnitName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int MaterialResourcesId { get; set; }
        public int QuantityTypeId { get; set; }
        public string CategoryDescription { get; set; }
        public string MaterialResourcesDescription { get; set; }
        public string QuantityTypeDescription { get; set; }
    }
}
