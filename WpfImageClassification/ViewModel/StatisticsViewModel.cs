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
        #region fields
        private ICommand _goToMenuViewCommand;
        private readonly Action _goToMenuViewAction;

        private double _cardboardAccuracy = 0.95;
        private double _cardboardPrecision;
        private double _cardboardRecall;
        private double _cardboardF1Score;

        private double _plasticAccuracy;
        private double _plasticPrecision;
        private double _plasticRecall;
        private double _plasticF1Score;

        private double _metalAccuracy;
        private double _metalPrecision;
        private double _metalRecall;
        private double _metalF1Score;

        private double _glassAccuracy;
        private double _glassPrecision;
        private double _glassRecall;
        private double _glassF1Score;

        private string _statisticsFilePath = "C:\\modelstatistics.txt";
        #endregion

        #region properties

        public ICommand GoToMenuViewCommand
        {
            get => _goToMenuViewCommand ??= new BaseCommand(GoToMenuView);
        }
        public double CardboardAccuracy
        {
            get => _cardboardAccuracy;
            set
            {
                _cardboardAccuracy = value;
                OnPropertyChanged();
            }
        }
        public double CardboardPrecision 
        { 
            get => _cardboardPrecision;
            set
            {
                _cardboardPrecision = value;
                OnPropertyChanged();
            }
        }
        public double CardboardRecall 
        { 
            get => _cardboardRecall;
            set
            {
                _cardboardRecall = value;
                OnPropertyChanged();
            }
        }
        public double CardboardF1Score
        {
            get => _cardboardF1Score;
            set
            {
                _cardboardF1Score = value;
                OnPropertyChanged();
            }
        }
        public double GlassPrecision 
        { 
            get => _glassPrecision;
            set
            {
                _glassPrecision = value;
                OnPropertyChanged();
            }
        }
        public double GlassRecall 
        { 
            get => _glassRecall;
            set
            {
                _glassRecall = value;
                OnPropertyChanged();
            }
        }
        public double GlassF1Score 
        { 
            get => _glassF1Score;
            set
            {
                _glassF1Score = value;
                OnPropertyChanged();
            }
        }
        public double MetalPrecision 
        { 
            get => _metalPrecision;
            set
            {
                _metalPrecision = value;
                OnPropertyChanged();
            }
        }
        public double MetalRecall 
        { 
            get => _metalRecall;
            set
            {
                _metalRecall = value;
                OnPropertyChanged();
            }
        }
        public double MetalF1Score 
        { 
            get => _metalF1Score;
            set
            {
                _metalF1Score = value;
                OnPropertyChanged();
            }
        }
        public double PlasticPrecision 
        { 
            get => _plasticPrecision;
            set
            {
                _plasticPrecision = value;
                OnPropertyChanged();
            }
        }
        public double PlasticRecall 
        { 
            get => _plasticRecall;
            set
            {
                _plasticRecall = value;
                OnPropertyChanged();
            }
        }
        public double PlasticF1Score 
        { 
            get => _plasticF1Score;
            set
            {
                _plasticF1Score = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public StatisticsViewModel(Action action)
        {
            _goToMenuViewAction = action;

            PopulateStatisticsFields();
        }

        private void PopulateStatisticsFields()
        {
            using(StreamReader reader = new StreamReader(_statisticsFilePath))
            {
                string data = reader.ReadLine();
                string[] separatedData;
                while (reader.ReadLine()!=null)
                {
                    separatedData = data.Split(',');
                    if (separatedData[0] == "Cardboard")
                    {
                        CardboardAccuracy = Double.Parse(separatedData[1]);
                        CardboardPrecision = Double.Parse(separatedData[2]);
                        CardboardRecall = Double.Parse(separatedData[3]);
                        CardboardF1Score = Double.Parse(separatedData[4]);
                    }
                    else if(separatedData[0] == "Plastic")
                    {
                        _plasticAccuracy = Double.Parse(separatedData[1]);
                        PlasticPrecision = Double.Parse(separatedData[2]);
                        PlasticRecall = Double.Parse(separatedData[3]);
                        PlasticF1Score = Double.Parse(separatedData[4]);
                    }
                    else if(separatedData[0] == "Metal")
                    {
                        _metalAccuracy = Double.Parse(separatedData[1]);
                        MetalPrecision = Double.Parse(separatedData[2]);
                        MetalRecall = Double.Parse(separatedData[3]);
                        MetalF1Score = Double.Parse(separatedData[4]);
                    }
                    else if(separatedData[0] == "Glass")
                    {
                        _glassAccuracy = Double.Parse(separatedData[1]);
                        GlassPrecision = Double.Parse(separatedData[2]);
                        GlassRecall = Double.Parse(separatedData[3]);
                        GlassF1Score = Double.Parse(separatedData[4]);
                    }
                    data = reader.ReadLine();
                }
            }
        }

        public void GoToMenuView(object obj)
        {
            _goToMenuViewAction?.Invoke();
        }
    }
}
