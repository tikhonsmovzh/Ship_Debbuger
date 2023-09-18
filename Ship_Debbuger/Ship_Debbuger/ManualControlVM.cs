using System.ComponentModel;

namespace Ship_Debbuger
{
    public class ManualControlVM : INotifyPropertyChanged
    {
        private readonly ShipManager _shipManager;
        public ManualControlVM(ShipManager shipManager)
        {
            _shipManager = shipManager;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}