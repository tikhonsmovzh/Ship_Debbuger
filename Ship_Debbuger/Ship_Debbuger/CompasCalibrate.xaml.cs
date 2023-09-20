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
        private SKBitmap PaletteBitmap => _paletteBitmap ?? (_paletteBitmap = new SKBitmap((int)_canvasView.Width * 2, (int)_canvasView.Height * 2));
        private readonly Timer _timer;
        // readonly Random _random;
        private readonly ShipManager _shipManager;
        private const int Scale = 2;
        private int maxX = int.MinValue;
        private int maxY = int.MinValue;
        private int minX = int.MaxValue;
        private int minY = int.MaxValue;

        public CompasCalibrate(ShipManager shipManager)
        {
            _shipManager = shipManager;
            InitializeComponent();
            _startButton.Text = "Начать";
            _timer = new Timer
            {

                Interval = 200
            };

            _timer.Elapsed += _timer_Elapsed;
        }

        private int _i;

        private readonly SKPaint _whitePaint = new SKPaint() { Color = new SKColor(255, 255, 255) };
        private readonly SKPaint _transparentPaint = new SKPaint() { Color = new SKColor(0, 0, 0) };

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var  compasParameters = _shipManager.GetCompasParameters();

            if (maxX < compasParameters.X)
                maxX = compasParameters.X;
            if (minX > compasParameters.X)
                minX = compasParameters.X;

            if (maxY < compasParameters.Y)
                maxY = compasParameters.Y;
            if (minY > compasParameters.Y)
                minY = compasParameters.Y;

            using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
            {               
                canvas.DrawCircle(convertToWorldX(compasParameters.X), convertToWorldY(compasParameters.Y), 3, _whitePaint);
                if (_i % 10 == 0)
                    _canvasView.InvalidateSurface();
            }

            _i++;
            _i = _i % int.MaxValue;
        }

        private int convertToWorldY(int y) => (PaletteBitmap.Height / 2 - y * Scale);
        private int convertToWorldX(int x) => (Scale * x + PaletteBitmap.Width / 2);

        private bool _isWorking;

        private void Button_Clicked(object sender, EventArgs e)
        {
            _isWorking = !_isWorking;
            _startButton.Text = _isWorking ? "Остановить" : "Начать";
            if (_isWorking)
            {
                maxX = int.MinValue;
                maxY = int.MinValue;
                minX = int.MaxValue;
                minY = int.MaxValue;

                using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
                {
                    canvas.DrawRect(0, 0, PaletteBitmap.Width, PaletteBitmap.Height, _transparentPaint);

                    canvas.DrawLine(PaletteBitmap.Width / 2, 0, PaletteBitmap.Width / 2, PaletteBitmap.Height, _whitePaint);
                    canvas.DrawLine(0, PaletteBitmap.Height / 2, PaletteBitmap.Width, PaletteBitmap.Height / 2, _whitePaint);

                    _canvasView.InvalidateSurface();
                }

                _saveButton.IsVisible = false;

                _timer.Start();
            }
            else
            {
                int maxXC = convertToWorldX(maxX);
                int minXC = convertToWorldX(minX);
                int maxYC = convertToWorldY(maxY);
                int minYC = convertToWorldY(minY);

                using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
                {
                    canvas.DrawLine(maxXC, 0, maxXC, PaletteBitmap.Height, _whitePaint);
                    canvas.DrawLine(minXC, 0, minXC, PaletteBitmap.Height, _whitePaint);

                    canvas.DrawLine(0, maxYC, PaletteBitmap.Width, maxYC, _whitePaint);
                    canvas.DrawLine(0, minYC, PaletteBitmap.Width, minYC, _whitePaint);

                    _canvasView.InvalidateSurface();
                }

                _saveButton.IsVisible = true;

                _timer.Stop();
            }


            //using (SKCanvas canvas = new SKCanvas(PaletteBitmap))
            //{

            //    canvas.DrawCircle(200, 300, 50, new SKPaint() { Color = new SKColor(255, 0, 0), IsStroke = true });
            //    //.DrawRect(0, 0, 50, 50, new SKPaint() { Color = new SKColor(255, 0,0) });
            //    _canvasView.InvalidateSurface();
            //}

        }

        private void Button_Clicked_Save(object sender, EventArgs e) 
        {
            _shipManager.WriteXY(maxX, minX, maxY, minY);
            _saveButton.IsVisible = false;
        }

        private void _canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.DrawBitmap(PaletteBitmap, 0, 0);
        }
    }
}