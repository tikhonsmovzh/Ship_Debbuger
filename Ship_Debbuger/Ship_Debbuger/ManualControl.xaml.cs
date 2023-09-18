using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ship_Debbuger
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManualControl : ContentPage
    {
       
       
        private readonly ShipManager _shipManager;
      

        public ManualControl(ShipManager shipManager)
        {
            _shipManager = shipManager;
            InitializeComponent();
            var vm = new ManualControlVM(shipManager);
            BindingContext = vm;

           
        }

      
    }
}