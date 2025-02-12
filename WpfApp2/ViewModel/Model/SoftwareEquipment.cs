namespace Vuzol.ViewModel.Model
{
    public class SoftwareEquipment
    {
        public int Id { get; set; }
        public int CountId { get; set; }
        public int MainPropertyFactoryNumber { get; set; }
        public string Description { get; set; }
        public SoftwareEquipment(int id, int mainPropertyFactoryNumber,string description)
        {
           this.Id = id;
           this.MainPropertyFactoryNumber = mainPropertyFactoryNumber;
           this.Description = description;
        }
    }
}
