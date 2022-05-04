using System.Collections.Generic;

public class qq<T>
{
    public Queue<T> ons = new Queue<T>();
    public Queue<T> offs = new Queue<T>();

    public T Off_To_On()
    {
        T obj = offs.Dequeue();
        ons.Enqueue(obj);
        return obj;
    }

    public T On_To_Off()
    {
        T obj = ons.Dequeue();
        offs.Enqueue(obj);
        return obj;
    }

    T _obj;
    public T DeqFromOff()
    {
        _obj = offs.Dequeue();
        return _obj;
    }

    public void EnqToOn()
    {
        ons.Enqueue(_obj);
    }

    public void Clear()
    {
        ons.Clear();
        offs.Clear();
    }
}
