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
        public ICommand GoToStatisticsViewCommand
        {
            get => _goToStatisticsViewCommand ??= new BaseCommand(GoToStatisticsView);
        }
        public string ClassificationResult
        {
            get => _classificationResult;
            set
            {
                _classificationResult = value;
                OnPropertyChanged();
            }
        }

        public MenuViewModel(Action action) 
        {
            _goToStatisticsViewAction = action;  
        }

        public void GoToStatisticsView(object obj)
        {
            _goToStatisticsViewAction?.Invoke();
        }

        private string _classificationResult = "picture is : ";

        public ICommand ClassifyImageCommand 
        {
            get => new BaseCommand(ClassifyImage);
        }

        public void ClassifyImage(object obj)
        {
            ClassificationResult = "picture is : Cat"; // Dummy result
        }
    }


}
