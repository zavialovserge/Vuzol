namespace Vuzol.ViewModel.Model
{
    public class Rank
    {
        public Rank(int id, string rankDescription)
        {
            Id = id;
            RankDescription = rankDescription;
        }
        public override string ToString()
        {
            return RankDescription;
        }
        public int Id { get; set; }
        public string RankDescription { get; set; }
    }
}
