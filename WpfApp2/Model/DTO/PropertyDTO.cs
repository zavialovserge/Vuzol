namespace Vuzol.Model.Db
{
    public class PropertyDTO
    {
        public int FactoryNumber { get; set; }
        public string? Name { get; set; }
        public int InventoryNumber { get; set; }
        public int InvoiceId { get; set; }
        public int BookId { get; set; }
        public int BookPage { get; set; }
        public int OrderBookId { get; set; }
        public int OrderBookPage { get; set; }
        public string? FormId { get; set; }
        public string? FormName { get; set; }
        public DateTime FormDate { get; set; }
        public int OrderId { get; set; }
        public int PropertyTypeId { get; set; }
        public string? Additionalnfo { get; set; }
        public DateTime DLM { get; set; }
        public int FIO_R { get; set; }
        public int FIO_I { get; set; }
        public string FIO_R_STR { get; set; }
        public string FIO_I_STR { get; set; }
        public string UnitName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
