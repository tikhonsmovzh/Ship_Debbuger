using System;
using System.Threading;

namespace Ship_Debbuger
{
    public class ShipManager
    {
        private static readonly byte[] RequestPoinData = new byte[] { 198 };
        private static readonly byte[] RequestZeroingData = new byte[] { 1 };
        private static readonly byte[] RequestSaveData = new byte[] { 2 };

        public ShipManager(IBlueToothHelper bluetoothHelper)
        {
           _bluetoothHelper = bluetoothHelper;
            _bluetoothHelper.Connect();
        }

        public void WriteZeroing(int azimut)
        {
            _bluetoothHelper.Write(RequestZeroingData);

            _bluetoothHelper.Write(BitConverter.GetBytes((short)azimut));
        }

        public All GetPoint()
        {
            All all = new All();
            _bluetoothHelper.Write(RequestPoinData);
            Thread.Sleep(200);
            var result = _bluetoothHelper.Read(16);

            short shiht(byte a, byte b) => (short)(a << 8 | b);

            all.point = new Point(shiht(result[0], result[1]),
                             shiht(result[4], result[5]));

            all.azimut = shiht(result[6], result[7]);

            all.l1 = shiht(result[8], result[9]);
            all.l2 = shiht(result[10], result[11]);

            all.positionX = shiht(result[12], result[13]);
            all.positionY = shiht(result[14], result[15]);

            return all;
        }

        private IBlueToothHelper _bluetoothHelper;

        public void WriteXY(int maxX, int minX, int maxY, int minY)
        {
            _bluetoothHelper.Write(RequestSaveData);

            _bluetoothHelper.Write(BitConverter.GetBytes((short)maxX));
            _bluetoothHelper.Write(BitConverter.GetBytes((short)minX));
            _bluetoothHelper.Write(BitConverter.GetBytes((short)maxY));
            _bluetoothHelper.Write(BitConverter.GetBytes((short)minY));
        }
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
        public Point point { get; set; }

        public int l1 { get; set; }
        public int l2 { get; set; }

        public int azimut { get; set; }

        public int positionX { get; set; }
        public int positionY { get; set; }
    }
}