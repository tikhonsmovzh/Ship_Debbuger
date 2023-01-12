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
            _bluetoothHelper.Connect();
        }

        public All GetPoint()
        {
            All all = new All();
            _bluetoothHelper.Write(RequestPoinData);
            Thread.Sleep(200);
            var result = _bluetoothHelper.Read(10);

            short shiht(byte a, byte b) => (short)(a << 8 | b);

            all.point = new Point(shiht(result[0], result[1]),
                             shiht(result[4], result[5]));
            
            all.l1 = shiht(result[6], result[7]);
            all.l2 = shiht(result[8], result[9]);

            return all;
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

    public class All
    {
        public Point point;

        public int l1;
        public int l2;
    }
}