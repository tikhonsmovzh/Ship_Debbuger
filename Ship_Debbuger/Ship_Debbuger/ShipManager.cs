using System;
using System.Threading;

namespace Ship_Debbuger
{
    public class ShipManager
    {
        private readonly byte[] RequestPoinData = new byte[] { 198 };
        private readonly byte[] RequestZeroingData = new byte[] { 1 };
        private readonly byte[] RequestSaveData = new byte[] { 2 };
        private readonly byte[] RequestZeroingGyroData = new byte[] { 3 };
        private readonly byte[] RequestFixationData = new byte[] { 4 };

        public ShipManager(IBlueToothHelper bluetoothHelper)
        {
            _bluetoothHelper = bluetoothHelper;
            _bluetoothHelper.Connect();
        }

        public void WriteZeroing() => _bluetoothHelper.Write(RequestZeroingData);

        public void WriteZeroingGyro() => _bluetoothHelper.Write(RequestZeroingGyroData);

        public void WriteFixationData() => _bluetoothHelper.Write(RequestFixationData);

        public All GetPoint()
        {
            All all = new All();
            _bluetoothHelper.Write(RequestPoinData);
            Thread.Sleep(200);
            var result = _bluetoothHelper.Read(20);

            short shiht(byte a, byte b) => (short)(a << 8 | b);

            all.point = new Point(shiht(result[0], result[1]),
                             shiht(result[4], result[5]));

            all.azimut = shiht(result[6], result[7]);

            all.l1 = shiht(result[8], result[9]);
            all.l2 = shiht(result[10], result[11]);

            all.positionX = shiht(result[12], result[13]);
            all.positionY = shiht(result[14], result[15]);

            all.Velocity = shiht(result[16], result[17]);

            all.Rot = shiht(result[18], result[19]);

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

        public int Rot { get; set; }
        public int Velocity { get; set; }
    }
}