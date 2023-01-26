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

        private bool _isShow;

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

        public string Azimut => $"азимут = {_all.azimut}";

        private void CalibrateCompas()
        {
            _isShow = false;
            _mainPage.ShowCompasCalibrate(_shipManager);
        }
        private void GetValue()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_isShow)
                    {
                        _all = _shipManager.GetPoint();

                        OnChanged(nameof(XVal));
                        OnChanged(nameof(YVal));
                        OnChanged(nameof(L1));
                        OnChanged(nameof(L2));
                        OnChanged(nameof(Azimut));

                        Task.Delay(200);
                    }
                }
            });

        }

        public ICommand CalibrateCompasCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Show() => _isShow = true;
    }
}