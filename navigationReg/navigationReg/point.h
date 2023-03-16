class point
{
    int x, y;

  public:
    point *Next;

    point(int x, int y)
    {
      this->x = x;
      this->y = y;

      Next = NULL;
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
