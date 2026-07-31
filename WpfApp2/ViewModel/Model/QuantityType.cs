namespace Vuzol.ViewModel.Model
{
    public class QuantityType
    {
        public QuantityType(int id, string description)
        {
            Id = id;
            Description = description;
        }
        public override string ToString()
        {
            return Description;
        }
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
