enum Dist { xPlus, yPlus, xMinus, yMinus };

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
