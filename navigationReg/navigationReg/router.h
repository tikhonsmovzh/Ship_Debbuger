class router
{
    const float lineCoef = 1;
    const float integrCoef = 0.5;

    int sumErr = 0;

    DynamicStructure *route;

    point *current;

    Servo servoForvard;
    Servo servoRear;

    const int distances = 20 * 20;

    Navigation *navigation;

  public:
    router(DynamicStructure *structure, Navigation *navigation)
    {
      route = structure;
      current = structure->Dequeue();

      this->navigation = navigation;
    }

    void Init()
    {
      servoRear.attach(12);
      servoForvard.attach(7);

      pinMode(12, OUTPUT);
      pinMode(7, OUTPUT);
    }

    void Update()
    {
      if (current == NULL)
      {
        current = route->Dequeue();

        return;
      }

      if (pow(current->GetY() - navigation->GetY(), 2) + pow(current->GetX() - navigation->GetX(), 2) < distances)
      {
        current = route->Dequeue();

        if (current == NULL)
          return;
      }

      int deegre = navigation->GetErorr(atan2(navigation->GetY() - current->GetY(), navigation->GetX() - current->GetX()));

      sumErr += deegre;

      servoRotate(deegre * lineCoef +  sumErr * integrCoef, &servoRear);
      servoRotate(-deegre * lineCoef - sumErr * integrCoef , &servoForvard);
    }
};
