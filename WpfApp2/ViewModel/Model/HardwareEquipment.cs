namespace Vuzol.ViewModel.Model
{
    public class HardwareEquipment
    {
        public int CountId { get; set; }
        public string MainInventoryNumber { get; set; }
        public string SubInventoryNumber { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public HardwareEquipment(string mainInventoryNumber, string subInventoryNumber, double quantity, string description)
        {
            this.SubInventoryNumber = subInventoryNumber;
            this.MainInventoryNumber = mainInventoryNumber;
            this.Description = description;
            this.Quantity = quantity;
        }
    }
}
