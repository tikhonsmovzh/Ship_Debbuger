using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ship_Debbuger
{
    public partial class MainPage : ContentPage
    {
        public MainPage(IBlueToothHelper bluetoothHelper)
        {
            
            InitializeComponent();
            BindingContext = new MainPageVM(new ShipManager(bluetoothHelper), this) ;
           
        }

        public async void ShowCompasCalibrate()
        {
            await Navigation.PushModalAsync(new CompasCalibrate());
        }
      
    }
}
