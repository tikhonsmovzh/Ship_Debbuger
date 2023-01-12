using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ship_Debbuger
{
    public class MainPageVM : INotifyPropertyChanged
    {
        //<Button Grid.Row="6" Text="Начать обмен" Command="{Binding GetCommand}"/>
        private readonly ShipManager _shipManager;
        private readonly MainPage _mainPage;
        private All _all = new All();

        public MainPageVM(ShipManager bluetoothHelper, MainPage mainPage)
        {
            CalibrateCompasCommand = new DelegateCommand(CalibrateCompas);
            _shipManager = bluetoothHelper;
            _mainPage = mainPage;
            GetValue();
        }


        public string XVal => $"X={_all.point.X}"; 
        public string YVal => $"Y={_all.point.Y}";

        public string L1 => $"растояние 1 = {_all.l1}";
        public string L2 => $"растояние 2 = {_all.l2}";
        
        private void CalibrateCompas()
        {
            _mainPage.ShowCompasCalibrate();
        }
        private void GetValue()
        {
            Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    _all  = _shipManager.GetPoint();

                    OnChanged(nameof(XVal));
                    OnChanged(nameof(YVal));
                    OnChanged(nameof(L1));
                    OnChanged(nameof(L2));

                    Task.Delay(200);
                }
            });

        }

        public ICommand CalibrateCompasCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
