using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace Ship_Debbuger
{
    public partial class MainPage : ContentPage
    {
        MainPageVM mainPageVM;

        public MainPage(IBlueToothHelper bluetoothHelper)
        {
            InitializeComponent();
            mainPageVM = new MainPageVM(new ShipManager(bluetoothHelper), this);
            BindingContext = mainPageVM;
            this.Appearing += Show;
        }

        private void Show(object sender, EventArgs e)
        {
            mainPageVM.Show();
        }

        public async void ShowCompasCalibrate(ShipManager shipManager)
        {
            await Navigation.PushModalAsync(new CompasCalibrate(shipManager));
        }
    }
}
