#include <Servo.h>

int sign(int val) {
  if (val < 0) return -1;
  if (val == 0) return 0;
  return 1;
}

int toIntB(byte a, byte b)
{
  int val = a << 8;
  return b | val;
}

void servoRotate(int degree1, Servo *serv0)
{
  if (abs(degree1) > 90)
    degree1 = sign(degree1) * 90;

  serv0->write(degree1 + 90);
}

void WriteInt(int val)
{
  Serial.write((byte)(val >> 8));
  Serial.write((byte)val);
}
