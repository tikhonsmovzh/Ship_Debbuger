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
    public partial class CompasCalibrate : ContentPage
    {
        private SKBitmap _paletteBitmap = null;
        private SKBitmap PaletteBitmap => _paletteBitmap ?? (_paletteBitmap = new SKBitmap((int)_canvasView.Width, (int)_canvasView.Height));
        private readonly Timer _timer;
        private readonly Random _random;
        public CompasCalibrate()
        {
            InitializeComponent();
            _startButton.Text =  "Начать";
            _timer = new Timer
            {
                Interval = 500
            };
            _timer.Elapsed += _timer_Elapsed;
            _random = new Random();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
            {

                canvas.DrawCircle(_random.Next(PaletteBitmap.Width-100)+50, _random.Next(PaletteBitmap.Height - 100) + 50, 50, new SKPaint() { Color = new SKColor(255, 0, 0), IsStroke = true });
                //.DrawRect(0, 0, 50, 50, new SKPaint() { Color = new SKColor(255, 0,0) });
                _canvasView.InvalidateSurface();
            }
        }

        private bool _isWorking;
        private void Button_Clicked(object sender, EventArgs e)
        {
            _isWorking = !_isWorking;
            _startButton.Text = _isWorking ? "Остановить" : "Начать";
            if (_isWorking)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }


            //using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
            //{

            //    canvas.DrawCircle(200, 300, 50, new SKPaint() { Color = new SKColor(255, 0, 0), IsStroke = true });
            //    //.DrawRect(0, 0, 50, 50, new SKPaint() { Color = new SKColor(255, 0,0) });
            //    _canvasView.InvalidateSurface();
            //}

        }

        private void _canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.DrawBitmap(PaletteBitmap, 0, 0);
        }
    }
}