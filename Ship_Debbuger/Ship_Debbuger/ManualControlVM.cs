using System;
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
            StopCommand = new DelegateCommand(() => UpdateValues(0, 0));
            ForvardCommand = new DelegateCommand(() => UpdateValues(new[] { LeftMotorValue + step, 255 }.Min(), new[] { RightMotorValue + step, 255 }.Min()));

            BackCommand = new DelegateCommand(() => UpdateValues(new[] { LeftMotorValue - step, -255 }.Max(), new[] { RightMotorValue - step, -255 }.Max()));

        }
        private void UpdateValues(int left, int right)
        {
            _isUpdated = true;
            LeftMotorValue = left;
            RightMotorValue = right;
            _shipManager.WriteMotorsValues(LeftMotorValue, RightMotorValue);
            _isUpdated = false;
        }

        private bool _isUpdated = false;
        public ICommand StopCommand { get; }
        public ICommand ForvardCommand { get; }
        public ICommand BackCommand { get; }

        public int LeftMotorValue
        {
            get => leftMotorValue;
            set
            {
                if (leftMotorValue == value) return;
                leftMotorValue = value;
                OnChanged(nameof(LeftMotorValue));
                if (_isUpdated) return;
                _shipManager.WriteMotorsValues(LeftMotorValue, RightMotorValue);
            }
        }

        public int RightMotorValue
        {
            get => rightMotorValue;
            set
            {
                if (rightMotorValue == value) return;
                rightMotorValue = value;
                OnChanged(nameof(RightMotorValue));
                if (_isUpdated) return;
                _shipManager.WriteMotorsValues(LeftMotorValue, RightMotorValue);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}