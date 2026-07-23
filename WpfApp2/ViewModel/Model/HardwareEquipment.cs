namespace Vuzol.ViewModel.Model
{
    public class HardwareEquipment
    {
        public int CountId { get; set; }
        public int MainPropertyFactoryNumber { get; set; }
        public int SubPropertyFactoryNumber { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public HardwareEquipment(int mainPropertyFactoryNumber, int subPropertyFactoryNumber, double quantity, string description)
        {
            this.SubPropertyFactoryNumber = subPropertyFactoryNumber;
            this.MainPropertyFactoryNumber = mainPropertyFactoryNumber;
            this.Description = description;
            this.Quantity = quantity;
        }
    }
}
