using System;
using System.Threading;

namespace Ship_Debbuger
{
    public class ShipManager
    {
        private static readonly byte[] RequestPoinData = new byte[] { 198 };

        public ShipManager(IBlueToothHelper bluetoothHelper)
        {
           _bluetoothHelper = bluetoothHelper;
        }

        public Point GetPoint()
        {
            _bluetoothHelper.Write(RequestPoinData);
            Thread.Sleep(200);
            var result = _bluetoothHelper.Read(4);

        }

        private IBlueToothHelper _bluetoothHelper;
    }
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}

