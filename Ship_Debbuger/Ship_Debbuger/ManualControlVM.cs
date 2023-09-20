using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Ship_Debbuger
{
    public class ManualControlVM : INotifyPropertyChanged
    {
        private readonly ShipManager _shipManager;
        private int leftMotorValue;
        private int rightMotorValue;
        const int step = 10;

        public ManualControlVM(ShipManager shipManager)
        {
            _shipManager = shipManager;
            _shipManager.StartManual();
            StopCommand = new DelegateCommand(() => { LeftMotorValue = 0;RightMotorValue = 0; });
            ForvardCommand = new DelegateCommand(() => { 
                LeftMotorValue = new[] {LeftMotorValue+step,255 }.Min();
                RightMotorValue = new[] { RightMotorValue + step, 255 }.Min();
            });
            BackCommand = new DelegateCommand(() =>
            {
                LeftMotorValue = new[] { LeftMotorValue - step, -255 }.Max();
                RightMotorValue = new[] { RightMotorValue - step, -255 }.Max();
            });
        }

        public ICommand StopCommand { get; }
        public ICommand ForvardCommand { get; }
        public ICommand BackCommand { get; }
        public int LeftMotorValue 
        {
            get => leftMotorValue;
            set 
            { 
                leftMotorValue = value;
                OnChanged(nameof(LeftMotorValue));
                _shipManager.WriteMotorsValues(LeftMotorValue,RightMotorValue);
            } 
        }

        public int RightMotorValue 
        { 
            get => rightMotorValue;
            set 
            {
                rightMotorValue = value;
                OnChanged(nameof(RightMotorValue));
                _shipManager.WriteMotorsValues(LeftMotorValue, RightMotorValue);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}