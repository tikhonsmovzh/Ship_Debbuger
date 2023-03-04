#include <Servo.h>
#include <Wire.h>
#include <EEPROM.h>
#include <TFLI2C.h>
#include "I2Cdev.h"
#include "MPU6050.h"

enum Dist { xPlus, yPlus, xMinus, yMinus };

const byte SensorP = 198;
const byte ZeroingCompassP = 1;
const byte saveP = 2;
const byte ZeroingGyroP = 3;
const byte distFixationP = 4;

const int timeDist = 100;
const int timeGyro = 20;

unsigned long tim = 0, gyroTim = 0, nowMil = 0;

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

int sign(int val) {
  if (val < 0) return -1;
  if (val == 0) return 0;
  return 1;
}

void WriteInt(int val)
{
  Serial.write((byte)(val >> 8));
  Serial.write((byte)val);
}

class point
{
    int x, y;

  public:
    point *Next;
  
    point(int x, int y)
    {
      this->x = x;
      this->y = y;
    }

    int GetX()
    {
      return x;
    }

    int GetY()
    {
      return y;
    }
};

class sector
{
    Dist dist[2];

  public:
    const int d = 5;
    int End;
    int Start;
    int deegre;

    sector(int deegres, Dist dist1, Dist dist2)
    {
      dist[0] = dist1;
      dist[1] = dist2;

      deegre = deegres;

      End = deegre + 45;

      Start = deegre - 45;

      if (Start < 0)
        Start += 360;
    }

    virtual bool isIncluded(int alpha)
    {
      if (Start <= alpha && alpha < End)
        return true;
      else
        return false;
    }

    virtual bool isIncludedEx(int alpha)
    {
      if (Start - d <= alpha && alpha < End + d)
        return true;
      else
        return false;
    }

    int GetX(int distance1, int distance2, int l, int h)
    {
      if (dist[0] == xMinus)
        return distance1;
      if (dist[0] == xPlus)
        return h - distance1;

      if (dist[1] == xMinus)
        return distance2;
      if (dist[1] == xPlus)
        return h - distance2;
    }

    int GetY(int distance1, int distance2, int l, int h)
    {
      if (dist[0] == yMinus)
        return distance1;
      if (dist[0] == yPlus)
        return l - distance1;

      if (dist[1] == yMinus)
        return distance2;
      if (dist[1] == yPlus)
        return l - distance2;
    }
};

class sector0: public sector
{
  public:
    sector0(int deegres, Dist dist1, Dist dist2): sector(deegres, dist1, dist2) {}

    bool isIncludedEx(int alpha)
    {
      if (alpha >= Start - d || alpha < End + d)
        return true;
      else
        return false;
    }

    bool isIncluded(int alpha)
    {
      if (alpha >= Start || alpha < End)
        return true;
      else
        return false;
    }
};

class Navigation
{
    sector* deegres[4] {new sector0(0, yPlus, xPlus), new sector(90, xPlus, yMinus), new sector(180, yMinus, xMinus), new sector(270, xMinus, yPlus)};

    int preferred, maxX, minX, maxY, minY, centerX, centerY, h, l, pool = 0;

    double rot = 0;

    unsigned long int timer = 0;

    const int SizeX = 26;
    const int SizeY = 59;

    const int addr = 0x1E;

    const int  Addr1 = 0x11;
    const int  Addr2 = TFL_DEF_ADR;

    const int GyroError = 379;

    TFLI2C tflI2C;

    MPU6050 accgyro;

    Servo servo;

    int GetValue()
    {
      Wire.requestFrom(addr, 6);

      for (int i = 0; i < 6; i++)
        buf[i] = Wire.read();

      int xC = toIntB(buf[0], buf[1]) - centerX;
      int yC = toIntB(buf[4], buf[5]) - centerY;

      int g = (int)(atan2(yC, xC) * 180 / PI) + 180 - pool;

      if (g < 0)
        g += 360;

      return g;
    }

    void InitCompass()
    {
      EEPROM.get(0, maxX);
      EEPROM.get(2, minX);
      EEPROM.get(4, maxY);
      EEPROM.get(6, minY);

      centerX = (maxX + minX) / 2;
      centerY = (maxY + minY) / 2;
    }

  public:
    int compass, x, y, gyro, gyroZ, distance1, distance2;

    byte buf[6];

    void Init()
    {
      Wire.beginTransmission(addr);
      Wire.write(0x00);
      Wire.write(0x70);
      Wire.write(0xA0);
      Wire.write(0x00);
      Wire.endTransmission();

      accgyro.initialize();

      servo.attach(10);

      pinMode(10, OUTPUT);

      InitCompass();

      EEPROM.get(8, pool);
      EEPROM.get(10, l);
      EEPROM.get(12, h);

      gyro = GetValue();
    }

    void SaveCompass()
    {
      InitCompass();
    }

    void ZeroingCompass()
    {
      pool = (compass + pool) % 360;

      EEPROM.put(8, pool);
    }

    void ZeroingGyro()
    {
      gyro = 0;
      rot = 0;
    }

    void UpdateDist()
    {
      Wire.beginTransmission(addr);
      Wire.write(0x03);
      Wire.endTransmission();

      tflI2C.getData(distance1, Addr2);
      tflI2C.getData(distance2, Addr1);

      x = deegres[preferred]->GetX(distance1, distance2, l, h);
      y = deegres[preferred]->GetY(distance1, distance2, l, h);

      compass = GetValue();

      if (!deegres[preferred]->isIncludedEx(gyro))
      {
        for (int i = 0; i < 4; i++)
        {
          if (deegres[i]->isIncluded(gyro))
          {
            preferred = i;

            break;
          }
        }
      }

      servoRotate(GetErorr(deegres[preferred]->deegre), &servo);
    }

    void GyroUpdate()
    {
      gyroZ = (accgyro.getRotationZ() + GyroError) / 131.0;

      unsigned long int nowMil = micros();

      rot -= (double)gyroZ * ((double)(nowMil - timer) / 1000000);

      timer = nowMil;

      if (rot < 0)
        rot += 360;

      if (rot >= 360)
        rot = 0;

      gyro = (int)rot;
    }

    void Fixation()
    {
      l = distance1;
      h = distance2;

      EEPROM.put(10, l);
      EEPROM.put(12, h);
    }

    float GetErorr(int alpha)
    {
      int e = gyro - alpha;

      int s = sign(e);

      int absE = abs(e);

      if (absE > 180)
        e = (360 - absE) * s * -1;

      return e;
    }
};

Navigation navigation;

class router
{
    int x, y;

    Servo servoForvard;
    Servo servoRear;

  public:
    router(int x, int y)
    {
      this->x = x;
      this->y = y;
    }

    void Init()
    {
      //servoRear.attach(12);
      //servoForvard.attach(7);

      //pinMode(12, OUTPUT);
      //pinMode(7, OUTPUT);
    }

    void Update()
    {
      int deegre = navigation.GetErorr(atan2(navigation.y - y, navigation.x - x));

      //servoRotate(deegre , &servoRear);
      //servoRotate(-deegre , &servoForvard);
    }
};

router Router(100, 300);

point d(5,5);

void setup() {
  Serial.begin(9600);

  Wire.begin();

  pinMode(4, INPUT);

  //pinMode(5, OUTPUT);
  //pinMode(6, OUTPUT);

  navigation.Init();
  Router.Init(); 
}

bool motorOn = false;

void loop() {
  nowMil = millis();

  if (tim < nowMil)
  {
    tim = nowMil + timeDist;

    navigation.UpdateDist();
    Router.Update();
  }

  if (gyroTim < nowMil)
  {
    gyroTim = nowMil + timeGyro;

    navigation.GyroUpdate();
  }

  if (Serial.available() > 0)
  {
    byte buf[1];

    Serial.readBytes(buf, 1);

    switch (buf[0])
    {
      case SensorP:
        {
          for (int i = 0; i < 6; i++)
            Serial.write(navigation.buf[i]);

          WriteInt(navigation.compass);
          WriteInt(navigation.distance1);
          WriteInt(navigation.distance2);
          WriteInt(navigation.x);
          WriteInt(navigation.y);
          WriteInt(navigation.gyroZ);
          WriteInt(navigation.gyro);

          break;
        }

      case saveP:
        {
          byte bufer[8];
          Serial.readBytes(bufer, 8);
          EEPROM.put(0, bufer);

          navigation.SaveCompass();

          break;
        }

      case ZeroingCompassP:
        navigation.ZeroingCompass();
        break;

      case ZeroingGyroP:
        navigation.ZeroingGyro();
        break;

      case distFixationP:
        navigation.Fixation();
        break;
    }
  }

  if (digitalRead(4) != motorOn)
  {
    if (digitalRead(4))
    {
      //analogWrite(5, 127);
      //analogWrite(6, 127);
    }
    else
    {
      //analogWrite(5, 0);
      //analogWrite(6, 0);
    }
  }

  motorOn = digitalRead(4);
}

/*void GetDynamicStructure(int count, point *(points[]))
{
  for(int i = count - 2; i >= 0; i--)
  {
    points[i].Next = &points[i + 1];
  }
}*/
