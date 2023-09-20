using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ship_Debbuger
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private readonly ShipManager _shipManager;
        private readonly MainPage _mainPage;
        private All _all = new All();

        private bool _isShow;

        public MainPageVM(ShipManager bluetoothHelper, MainPage mainPage)
        {
            CalibrateCompasCommand = new DelegateCommand(CalibrateCompas);
            ManualControlCommand = new DelegateCommand(ManualControl);
            ZeroingCommand = new DelegateCommand(ZeroingCompas);
            ZeroingGyroCommand = new DelegateCommand(ZeroingGyro);
            FixationCommand = new DelegateCommand(Fixation);
            _shipManager = bluetoothHelper;
            _mainPage = mainPage;
            GetValue();
        }

        public string Lactitude => $"   Широта = {(double)_all.Lactitude / 1000000}";
        public string Longtitude => $"  Долгота = {(double)_all.Longtitude / 1000000}";


        public string Azimut => $"   азимут = {_all.Azimut}";


        private void CalibrateCompas()
        {
            _isShow = false;
            _mainPage.ShowCompasCalibrate(_shipManager);
        }

        private void ManualControl()
        {
            _isShow = false;
            _mainPage.ShowManualControl(_shipManager);
        }

        private void ZeroingCompas() => _shipManager.WriteZeroing();

        private void ZeroingGyro() => _shipManager.WriteZeroingGyro();

        private void Fixation() => _shipManager.WriteFixationData();

        private void GetValue()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_isShow)
                    {
                        _all = _shipManager.GetPoint();

                        OnChanged(nameof(Longtitude));
                        OnChanged(nameof(Lactitude));
                        OnChanged(nameof(Azimut));

                        Task.Delay(200);
                    }
                }
            });

        }

        public ICommand CalibrateCompasCommand { get; }
        public ICommand ManualControlCommand { get; }
        public ICommand ZeroingCommand { get; }
        public ICommand ZeroingGyroCommand { get; }
        public ICommand FixationCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Show()
        {
            _isShow = true;
            _shipManager.StopManualMode();
        }
    }
}