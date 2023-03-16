#include <Wire.h>
#include <EEPROM.h>
#include "point.h"
#include "publicMethods.h"
#include "sector.h"
#include "navigation.h"
#include "DynamicStructure.h"
#include "router.h"

const byte SensorP = 198;
const byte ZeroingCompassP = 1;
const byte saveP = 2;
const byte ZeroingGyroP = 3;
const byte distFixationP = 4;

const int timeDist = 100;
const int timeGyro = 20;

const int workSpeed = 128;

unsigned long tim = 0, gyroTim = 0, nowMil = 0;

Navigation navigation;

DynamicStructure points;

router Router(&points, &navigation);

bool previousKey;

void setup() {
  points.Enqueu(&point(100, 300));

  Serial.begin(9600);

  Wire.begin();

  pinMode(4, INPUT);

  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);

  navigation.Init();
  Router.Init();

  previousKey = digitalRead(4);
}

void loop() {
  nowMil = millis();

  if (gyroTim < nowMil)
  {
    gyroTim = nowMil + timeGyro;

    navigation.GyroUpdate();
  }

  if (tim < nowMil)
  {
    tim = nowMil + timeDist;

    navigation.UpdateDist();

    bool nowKey = digitalRead(4);

    if (nowKey)
      Router.Update();

    if (nowKey != previousKey)
    {
      if (nowKey)
      {
        analogWrite(5, workSpeed);
        digitalWrite(6, HIGH);
      }
      else
      {
        analogWrite(5, 0);
        digitalWrite(6, LOW);
      }
    }

    previousKey = nowKey;
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

          WriteInt(navigation.GetCompass());
          WriteInt(navigation.GetDistance1());
          WriteInt(navigation.GetDistance2());
          WriteInt(navigation.GetX());
          WriteInt(navigation.GetY());
          WriteInt(navigation.GetGyroSpeed());
          WriteInt(navigation.GetGyro());

          break;
        }

      case saveP:
        {
          byte bufer[8];
          Serial.readBytes(bufer, 8);
          EEPROM.put(navigation.SaveBaseData, bufer);

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
}
