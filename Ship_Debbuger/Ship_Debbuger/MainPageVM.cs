using System.ComponentModel;
using System.Windows.Input;

namespace Ship_Debbuger
{
    public class MainPageVM : INotifyPropertyChanged
    {
      
        private readonly ShipManager _shipManager;
        private Point _point = new Point(0, 0);

        public MainPageVM(ShipManager bluetoothHelper)
        {
            GetCommand = new DelegateCommand(GetValue);
            _shipManager = bluetoothHelper;
        }


        public string XVal => $"X={_point.X}"; 
        public string YVal => $"Y={_point.Y}"; 
        
        private void GetValue()
        {
             _point=_shipManager.GetPoint();
         
            OnChanged(nameof(XVal));
            OnChanged(nameof(YVal));

        }

        public ICommand GetCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
      
    }
}
