namespace Vuzol.ViewModel.Model
{
    public class SoftwareEquipment
    {
        public int Id { get; set; }
        public int CountId { get; set; }
        public double Quantity { get; set; }
        public string MainInventoryNumber { get; set; }
        public string Description { get; set; }
        public SoftwareEquipment(int id, string mainInventoryNumber, string description, double quantity)
        {
            this.Id = id;
            this.MainInventoryNumber = mainInventoryNumber;
            this.Description = description;
            this.Quantity = quantity;
        }
    }
}
