using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageClassification.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel 
        { 
            get => _currentViewModel;
            set
            { 
                _currentViewModel = value; 
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel() 
        {
            CurrentViewModel = new MenuViewModel(GoToStatisticsView);
        }

        public void GoToStatisticsView()
        {
            CurrentViewModel = new StatisticsViewModel(GoToMenuView);
        }

        public void GoToMenuView()
        {
            CurrentViewModel = new MenuViewModel(GoToStatisticsView);
        }
    }
}
