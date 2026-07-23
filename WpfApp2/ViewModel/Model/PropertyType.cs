namespace Vuzol.ViewModel.Model
{
    public class PropertyType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PropertyType(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
