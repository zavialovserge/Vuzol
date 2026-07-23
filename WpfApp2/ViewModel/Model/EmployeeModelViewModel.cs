using System.ComponentModel;

namespace Vuzol.ViewModel.Model
{
    public class EmployeeModelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id { get; set; }
        private string _firstName { get; set; }
        private string _lastName { get; set; }
        private string _fatherName { get; set; }
        private int _rank { get; set; }
        private string _position { get; set; }
        private string _rankDescription { get; set; }
        private string _unitDescription { get; set; }
        private List<string> _rankList { get; set; }
        private List<string> _unitList { get; set; }
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(_id));
            }
        }
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(_firstName));
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(_lastName));
            }
        }
        public string FatherName
        {
            get { return _fatherName; }
            set
            {
                _fatherName = value;
                OnPropertyChanged(nameof(_fatherName));
            }
        }
        public int Rank
        {
            get { return _rank; }
            set
            {
                _rank = value;
                OnPropertyChanged(nameof(_rank));
            }
        }
        public string Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged(nameof(_position));
            }
        }
        public string RankDescription
        {
            get { return _rankDescription; }
            set
            {
                _rankDescription = value;
                OnPropertyChanged(nameof(_rankDescription));
            }
        }
        public string UnitDescription
        {
            get { return _unitDescription; }
            set
            {
                _unitDescription = value;
                OnPropertyChanged(nameof(_unitDescription));
            }
        }
        public List<string> RankList
        {
            get { return _rankList; }
            set
            {
                _rankList = value;
                OnPropertyChanged(nameof(_rankList));
            }
        }
        public List<string> UnitList
        {
            get { return _unitList; }
            set
            {
                _unitList = value;
                OnPropertyChanged(nameof(_unitList));
            }
        }
    }
}
