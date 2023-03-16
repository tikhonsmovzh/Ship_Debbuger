class DynamicStructure
{
    point *first, *last;

  public:
    point* Dequeue()
    {
      point *saveFirst = first;

      if (first != NULL)
      {
        first = first->Next;

        if (first == NULL)
          last = NULL;
      }

      return saveFirst;
    }

    void Enqueu(point *data)
    {
      if (first == NULL)
      {
        first = data;
        last = data;

        return;
      }

      last->Next = data;

      last = data;
    }
};
