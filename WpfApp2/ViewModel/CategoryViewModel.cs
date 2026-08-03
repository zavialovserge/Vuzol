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
    public class CategoryViewModel : BaseViewModel
    {
        public ObservableCollection<Category> CategoryList { get; set; }
        public Category SelectedCategory { get; set; }
        public ICommand HomeCommand { get; }
        private RelayCommand _addCategoryCommand;
        private RelayCommand _editCategoryCommand;
        private RelayCommand _delCategoryCommand;
        public CategoryViewModel(NavigationProperty navigationProperty)
        {
            HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
               () => new HomeViewModel(navigationProperty));
            CategoryList = new ObservableCollection<Category>(CategoryData.GetAllCategorys());
            SelectedCategory = CategoryList.First();
        }
        public ICommand DelCategoryCommand
        {
            get
            {
                return _delCategoryCommand ?? (_delCategoryCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       CategoryData.DeleteFromDb(SelectedCategory);
                       CategoryList.Remove(SelectedCategory);

                   }));
            }
        }
        public ICommand AddCategoryCommand
        {
            get
            {
                return _addCategoryCommand ?? (_addCategoryCommand = new RelayCommand(
                   x =>
                   {
                       string decription = GetCategoryName(string.Empty);
                       if (string.IsNullOrEmpty(decription)) return;
                       Category Category = new Category(0, decription);
                       CategoryData.InsertCategory(Category);
                       RefreshCollection();

                   }));
            }
        }
        public ICommand EditCategoryCommand
        {
            get
            {
                return _editCategoryCommand ?? (_editCategoryCommand = new RelayCommand(
                   x =>
                   {
                       string name = GetCategoryName(SelectedCategory.Description);
                       if (string.IsNullOrEmpty(name)) return;
                       SelectedCategory.Description = name;
                       CategoryData.EditIEank(SelectedCategory);
                       RefreshCollection();
                   }));
            }
        }
        private string GetCategoryName(string name)
        {
            InputDialogSample inputDialog =
                       new InputDialogSample("Введіть категорію", name);
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
        private void RefreshCollection()
        {
            CategoryList.Clear();
            var categoryData = CategoryData.GetAllCategorys();
            foreach (var Category in categoryData)
            {
                CategoryList.Add(Category);
            }
        }
    }
}
