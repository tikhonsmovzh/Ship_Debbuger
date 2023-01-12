using System.Collections.Generic;
using Android.Bluetooth;
using Java.Util;
using System;
using System.Linq;
using System.Threading;

namespace Ship_Debbuger.Droid
{
    public class BluetoothHelper: IBlueToothHelper
    {
        private bool _connected = false;

        private BluetoothSocket _soket;

        private const string DeviceName = "HC-05";

        public void Connect()
        {
            if (_connected) return;

            var adapter = Android.Bluetooth.BluetoothAdapter.DefaultAdapter;
            if (adapter == null) return;
            if (!adapter.IsEnabled)
            {
                adapter.Enable();
            }
            while (!adapter.IsEnabled) Thread.Sleep(100);

            var devices = adapter.BondedDevices;
            var btDevice1 = devices.Where(d => d.Name == DeviceName).SingleOrDefault();
            if (btDevice1 == null)
            {
                throw new Exception($"Not found {DeviceName}");
            }
            _soket = btDevice1.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
            adapter.CancelDiscovery();
            try
            {
                _soket.Connect();
            }
            catch (Exception)
            {
                return;
            }

            _connected = true;


        }

        public void Write(byte[] data)
        {
            Connect();
            _soket.OutputStream.Write(data, 0, data.Length);
        }

        public byte[] Read(int countBytes)
        {
            byte[] buf = new byte[countBytes];

            Connect();

            _soket.InputStream.Read(buf, 0, buf.Length);

            return buf;
        }
    }
}