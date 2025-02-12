namespace Vuzol.ViewModel.Model
{
    public class PropertyStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PropertyStatus(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
