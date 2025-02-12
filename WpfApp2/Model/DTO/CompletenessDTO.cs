namespace Vuzol.Model.Db
{
    public class CompletenessDTO
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int FactoryNumber { get; set; }
        public int InventoryNumber { get; set; }
        public DateTime DLM { get; set; }
        public DateTime Date_d { get; set; }
        public string? Reason { get; set; }
    }
}
