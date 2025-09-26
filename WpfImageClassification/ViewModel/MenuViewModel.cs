using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImageClassification.ViewModel
{
    public class MenuViewModel : BaseViewModel
    {
        private ICommand _goToStatisticsViewCommand;
        private readonly Action _goToStatisticsViewAction;

        public MenuViewModel(Action action) 
        {
            _goToStatisticsViewAction = action;  
        }

        public ICommand GoToStatisticsViewCommand
        {
            get => _goToStatisticsViewCommand ??= new BaseCommand(GoToStatisticsView);
        }

        public void GoToStatisticsView(object obj)
        {
            _goToStatisticsViewAction?.Invoke();
        }


    }

}
