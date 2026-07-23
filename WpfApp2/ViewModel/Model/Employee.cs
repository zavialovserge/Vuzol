namespace Vuzol.ViewModel.Model
{
    public class Employee
    {
        public Employee(int id, string firstName, string lastName,
                        string fatherName, int rank, string position,
                        string RankDescription, int unit, string UnitName)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.FatherName = fatherName;
            this.Rank = rank;
            this.Position = position;
            this.RankDescription = RankDescription;
            this.Unit = unit;
            this.UnitName = UnitName;
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public int Rank { get; set; }
        public int Unit { get; set; }
        public string Position { get; set; }
        public string RankDescription { get; set; }

        public string UnitName { get; set; }
    }
}
