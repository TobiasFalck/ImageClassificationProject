using WpfImageClassification; 
using WpfImageClassification.ViewModel;
using WpfImageClassification.View;
using System.Windows.Controls;
namespace WpfAppTest
{
    public class UnitTest1
    {
        
        [Fact]
        public void TestMenuViewModel()
        {
            //arrange
            BaseViewModel actual = new MainWindowViewModel();
            BaseViewModel expected;
            
            //act
            actual = actual.CurrentViewModel;
            expected = new MenuViewModel();
            
            //assert
            Assert.Equal(expected.GetType(), actual.GetType());
        }

        [Fact]
        public void TestStatViewModel()
        {
            //arrange
            BaseViewModel actual = new MainWindowViewModel();
            BaseViewModel expected;

            //act
            actual = actual.CurrentViewModel;
            expected = new StatisticsViewModel();
            
            //assert
            Assert.Equal(expected.GetType(), actual.GetType());
        }

        [Fact]
        public void TestMenuView()
        {
            //arrange
            UserControl actual; 
            UserControl expected;

            //act
            actual = MainWindow.CurrentView as UserControl;
            expected = new MenuView();

            //assert
            Assert.Equal(expected.GetType(), actual.GetType());
        }

        [Fact]
        public void TestStatView()
        {
            //arrange
            UserControl actual;
            UserControl expected;

            //act
            actual = MainWindow.CurrentView as UserControl;
            expected = new StatisticsView();

            //assert
            Assert.Equal(expected.GetType(), actual.GetType());
        }
    }
}
