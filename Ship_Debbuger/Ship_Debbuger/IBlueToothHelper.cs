using System;

namespace Ship_Debbuger
{
    public interface IBlueToothHelper
    {
        void Connect();
        void Write(byte[] data);
        byte[] Read(int countBytes);
    }
}