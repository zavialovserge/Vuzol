using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.View;
using Vuzol.ViewModel.Command;
using Vuzol.ViewModel.Model;

namespace Vuzol.ViewModel
{
    public class RankViewModel:BaseViewModel
    {
        public ObservableCollection<Rank> RankList { get; set; }
        public Rank SelectedRank { get; set; }
        public ICommand HomeCommand { get; }
        private RelayCommand _addRankCommand;
        private RelayCommand _editRankCommand;
        private RelayCommand _delRankCommand;
        public RankViewModel(NavigationProperty navigationProperty)
        {

            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            RankList = new ObservableCollection<Rank>(RankData.getAllRanks());
            SelectedRank = RankList.First();
        }
        public ICommand DelRankCommand
        {
            get
            {
                return _delRankCommand ?? (_delRankCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       RankData.DeleteFromDb(SelectedRank);
                       RankList.Remove(SelectedRank);

                   }));
            }
        }
        public ICommand AddRankCommand
        {
            get
            {
                return _addRankCommand ?? (_addRankCommand = new RelayCommand(
                   x =>
                   {
                       string decription = GetRankName(string.Empty);
                       if (string.IsNullOrEmpty(decription)) return;
                       Rank rank = new Rank(0, decription);
                       RankData.InsertRank(rank);
                       RefreshCollection();

                   }));
            }
        }
        public ICommand EditRankCommand
        {
            get
            {
                return _editRankCommand ?? (_editRankCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetRankName(SelectedRank.RankDescription);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedRank.RankDescription = name;
                       RankData.EditIEank(SelectedRank);
                       RefreshCollection();
                   }));
            }
        }
        private string GetRankName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть звання", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            RankList.Clear();
            var rankData = RankData.getAllRanks();
            foreach (var rank in rankData)
            {
                RankList.Add(rank);
            }
        }
    }
}
