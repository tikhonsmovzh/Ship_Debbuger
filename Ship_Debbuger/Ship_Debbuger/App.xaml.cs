using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ship_Debbuger
{
    public partial class App : Application
    {
        public App(IBlueToothHelper bluetoothHelper)
        {
            InitializeComponent();

            MainPage = new MainPage(bluetoothHelper);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
