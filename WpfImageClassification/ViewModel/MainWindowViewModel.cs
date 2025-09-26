using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageClassification.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private BaseViewModel _curentViewModel;

        public BaseViewModel CurentViewModel 
        { 
            get => _curentViewModel;
            set
            { 
                _curentViewModel = value; 
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel() 
        {
            CurentViewModel = new MenuViewModel(GoToStatisticsView);
        }

        public void GoToStatisticsView()
        {
            CurentViewModel = new StatisticsViewModel(GoToMenuView);
        }

        public void GoToMenuView()
        {
            CurentViewModel = new MenuViewModel(GoToStatisticsView);
        }
    }
}
