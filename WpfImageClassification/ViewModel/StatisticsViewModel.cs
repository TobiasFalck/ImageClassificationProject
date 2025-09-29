using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfImageClassification.ViewModel
{
    public class StatisticsViewModel : BaseViewModel
    {
        private ICommand _goToMenuViewCommand;
        private readonly Action _goToMenuViewAction;
        private double _accuracy = 0.95;
        private double _precision;
        private double _recall;
        private double _f1Score;

        private string _statisticsFilePath = "C:\\Users\\jeppe\\source\\repos\\ImageClassificationProject\\ImageClassificationProject\\bin\\Debug\\net9.0-windows\\data\\modelstatistics.txt";

        public ICommand GoToMenuViewCommand
        {
            get => _goToMenuViewCommand ??= new BaseCommand(GoToMenuView);
        }
        public double Accuracy
        {
            get => _accuracy;
            set
            {
                _accuracy = value;
                OnPropertyChanged();
            }
        }
        public double Precision { get => _precision; set => _precision = value; }
        public double Recall { get => _recall; set => _recall = value; }
        public double F1Score { get => _f1Score; set => _f1Score = value; }

        public StatisticsViewModel(Action action)
        {
            _goToMenuViewAction = action;
        }

        public void GoToMenuView(object obj)
        {
            _goToMenuViewAction?.Invoke();
        }
    }
}
