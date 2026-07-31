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
        public class MaterialResourcesViewModel : BaseViewModel
        {
            public ObservableCollection<MaterialResources> MaterialResourcesList { get; set; }
            public MaterialResources SelectedMaterialResources { get; set; }
            public ICommand HomeCommand { get; }
            private RelayCommand _addMaterialResourcesCommand;
            private RelayCommand _editMaterialResourcesCommand;
            private RelayCommand _delMaterialResourcesCommand;
            public MaterialResourcesViewModel(NavigationProperty navigationProperty)
            {
                HomeCommand = new NavigateCommand<HomeViewModel>(navigationProperty,
                   () => new HomeViewModel(navigationProperty));
                MaterialResourcesList = new ObservableCollection<MaterialResources>(MaterialResourcesData.getAllMaterialResourcess());
                SelectedMaterialResources = MaterialResourcesList.First();
            }
            public ICommand DelMaterialResourcesCommand
            {
                get
                {
                    return _delMaterialResourcesCommand ?? (_delMaterialResourcesCommand = new RelayCommand(
                       x =>
                       {
                           if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                               MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                           {
                               return;
                           }
                           MaterialResourcesData.DeleteFromDb(SelectedMaterialResources);
                           MaterialResourcesList.Remove(SelectedMaterialResources);

                       }));
                }
            }
            public ICommand AddMaterialResourcesCommand
            {
                get
                {
                    return _addMaterialResourcesCommand ?? (_addMaterialResourcesCommand = new RelayCommand(
                       x =>
                       {
                           string decription = GetMaterialResourcesName(string.Empty);
                           if (string.IsNullOrEmpty(decription)) return;
                           MaterialResources MaterialResources = new MaterialResources(0, decription);
                           MaterialResourcesData.InsertMaterialResources(MaterialResources);
                           RefreshCollection();

                       }));
                }
            }
            public ICommand EditMaterialResourcesCommand
            {
                get
                {
                    return _editMaterialResourcesCommand ?? (_editMaterialResourcesCommand = new RelayCommand(
                       x =>
                       {
                           string name = GetMaterialResourcesName(SelectedMaterialResources.Description);
                           if (string.IsNullOrEmpty(name)) return;
                           SelectedMaterialResources.Description = name;
                           MaterialResourcesData.EditIEank(SelectedMaterialResources);
                           RefreshCollection();
                       }));
                }
            }
            private string GetMaterialResourcesName(string name)
            {
                InputDialogSample inputDialog =
                           new InputDialogSample("Введіть звання", name);
                if (inputDialog.ShowDialog() == false
                    || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

                return inputDialog.Answer;
            }
            private void RefreshCollection()
            {
                MaterialResourcesList.Clear();
                var materialResourcesData = MaterialResourcesData.getAllMaterialResourcess();
                foreach (var MaterialResources in materialResourcesData)
                {
                    MaterialResourcesList.Add(MaterialResources);
                }
            }
        }
}
