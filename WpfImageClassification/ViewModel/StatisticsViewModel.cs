using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImageClassification.ViewModel
{
    public class StatisticsViewModel : BaseViewModel
    {
        private ICommand _goToMenuViewCommand;
        private readonly Action _goToMenuViewAction;

        public StatisticsViewModel(Action action) 
        {
            _goToMenuViewAction = action;
        }
    }
}
