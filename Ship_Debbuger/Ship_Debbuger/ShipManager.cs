﻿using System;
using System.Threading;

namespace Ship_Debbuger
{
    public class ShipManager
    {
        private readonly byte[] RequestPoinData = new byte[] { 198 };
        private readonly byte[] RequestCompasParametesData = new byte[] { 199 };
        private readonly byte[] RequestZeroingData = new byte[] { 1 };
        private readonly byte[] RequestSaveData = new byte[] { 2 };
        private readonly byte[] RequestZeroingGyroData = new byte[] { 3 };
        private readonly byte[] RequestFixationData = new byte[] { 4 };
        private readonly byte[] RequestStartManualMode = new byte[] { 5 };
        private readonly byte[] RequestSetMotorsValues = new byte[] { 7 };
        private readonly byte[] RequestStopManualMode = new byte[] { 8 };

        public ShipManager(IBlueToothHelper bluetoothHelper)
        {
            _bluetoothHelper = bluetoothHelper;
            _bluetoothHelper.Connect();
        }

        public void WriteZeroing() => _bluetoothHelper.Write(RequestZeroingData);

        public void WriteZeroingGyro() => _bluetoothHelper.Write(RequestZeroingGyroData);

        public void WriteFixationData() => _bluetoothHelper.Write(RequestFixationData);

        public void StopManualMode() => _bluetoothHelper.Write(RequestStopManualMode);

        private short shiht(byte a, byte b) => (short)(a << 8 | b);
        private uint shiftLong(byte a, byte b, byte c, byte d) => (uint)(a << 24 | b << 16 | c << 8 | d);

        public All GetPoint()
        {
            All all = new All();
            _bluetoothHelper.Write(RequestPoinData);
            Thread.Sleep(200);
            var result = _bluetoothHelper.Read(10);

          

            all.Lactitude = shiftLong(result[0], result[1], result[2], result[3]);
            all.Longtitude = shiftLong(result[4], result[5], result[6], result[7]);

            all.Azimut = shiht(result[8], result[9]);                  

         
            return all;
        }
        public CompasParameters GetCompasParameters()
        {
            _bluetoothHelper.Write(RequestCompasParametesData);
            var result = _bluetoothHelper.Read(6);
            return new CompasParameters(shiht(result[0], result[1]), shiht(result[4], result[5]));


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

        internal void StartManual()
        {
           
            _bluetoothHelper.Write(RequestStartManualMode);
        }

        internal void WriteMotorsValues(int leftMotorValue, int rightMotorValue)
        {
           
            _bluetoothHelper.Write(RequestSetMotorsValues);
            _bluetoothHelper.Write(BitConverter.GetBytes((short)leftMotorValue));
            _bluetoothHelper.Write(BitConverter.GetBytes((short)rightMotorValue));

        }
    }

    public class CompasParameters
    {
        public CompasParameters(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    public class All
    {
            //public Point point { get; set; }

            public long Lactitude { get; set; }
            public long Longtitude { get; set; }

            public int Azimut { get; set; }

            public int CompasX { get; set; }
            public int CompasY { get; set; }

                }
    }
