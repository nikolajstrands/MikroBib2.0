using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LibraryClient.Services;

namespace LibraryClient.ViewModels
{
    // Denne klasse definerer et View, hvor der kan navigeres mellem en række underliggende views

    public class NavigationViewModel : BaseViewModel
    {

        // Det nuværende synlige view
        private BaseViewModel currentPageViewModel;
        public BaseViewModel CurrentPageViewModel
        {
            get
            {
                return currentPageViewModel;
            }
            set
            {
                if (currentPageViewModel != value)
                {
                    currentPageViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        // En liste over de mulige views der kan navigeres mellem
        private ObservableCollection<BaseViewModel> pageViewModels;
        public ObservableCollection<BaseViewModel> PageViewModels
        {
            get
            {
                if (pageViewModels == null)
                    pageViewModels = new ObservableCollection<BaseViewModel>();

                return pageViewModels;
            }
        }

        // Constructors
        public NavigationViewModel(AppState appState)
            : base(appState)
        {
        } 
        public NavigationViewModel() { }

        
        
        // Kommando til at skifte til et nyt view (som kommer fra paramtren)
        private ICommand changePageCommand;    
        public ICommand ChangePageCommand
        {
            get
            {
                if (changePageCommand == null)
                {

                    changePageCommand = new RelayCommand(
                        p => ChangeViewModel((BaseViewModel)p),
                        p => (p is BaseViewModel) && ((BaseViewModel)p).IsUserAuthorized(AppState.User) && AppState.CanShiftView == true
                       //p => true
                       );
                }

                return changePageCommand;
            }
        }

        // Hjælpefunktion der skifter det aktive view
        private void ChangeViewModel(BaseViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {             
                var oldVM = PageViewModels.FirstOrDefault(vm => vm.GetType() == viewModel.GetType());

                if(oldVM != null) {

                    oldVM = viewModel;
                }
                else
                {
                    PageViewModels.Add(viewModel);
                }                                      
            }              
            CurrentPageViewModel = viewModel;
        }




}

}
