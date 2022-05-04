
public class arr<T> where T : new() 
{
    public int Num = 0;
    int _max = 0;
    T[] _objs = null;
    
    public bool IsFull { get { return Num == _max; } }
    public int Max { get { return _max; } }
    public T this[int i] { get { return _objs[i]; } set { _objs[i] = value; } }
    public T this[short i] { get { return _objs[i]; } set { _objs[i] = value; } }

    public T Peek { get { return _objs[Num-1]; } }
    public arr(int max)
    {
        _max = max;
        _objs = new T[_max];
        for(int i=0; i<max; ++i)
            _objs[i] = new T();
    }

    public void Reset_Add(T obj)
    {
        _objs[0] = obj;
        Num = 1;
    }
    public void Clear()
    {
        Num = 0;
    }
    
    public void CountNum()
    {
        ++Num;
    }
    public void Add(T obj)
    {
        if (Num < _max)
            _objs[Num++] = obj;
    }
    public void Remove(int idx)
    {
        if (idx < 0 || idx >= Num)
            return;
        _objs[idx] = _objs[Num - 1];
        --Num;
    }
    
    public void Copy(ref arr<T> by)
    {
        for(int i=0; i<by.Num; ++i)
            _objs[i] = by[i];
        Num = by.Num;
    }

    public void CopyOfReverse(ref arr<T> by)
    {
        for(int i=0; i<by.Num; ++i)
        {
            _objs[i] = by[by.Num-i-1];
        }
        Num = by.Num;
    }

    public void Reverse()
    {
        int half = Num>>1;
        for(int i=0; i<half; ++i)
        {
            var tmp = _objs[i];
            _objs[i] = _objs[Num - (1 + i)];
            _objs[Num - (1 + i)] = tmp;

        }
    }
}

public class arr2d<T> where T : class, new()
{
    public arr2d(int maxX_, int maxY_) 
    {
        _maxX = maxX_; _maxY = maxY_;
        _objs = new T[maxX_, maxY_];
        for (int x = 0; x < maxX_; ++x)
                for (int y = 0; y < maxY_; ++y)
                    _objs[x, y] = new T();
    }

    public void Clear()
    {
        for (int x = 0; x < _maxX; ++x)
            for (int y = 0; y < _maxY; ++y)
                    _objs[x, y] = null;
    }

    public bool IsEmpty(int x, int y) { return null == _objs[x, y]; }
    public bool IsOutIdx(int x, int y) { return (x < 0 || y < 0 || x >= _maxX || y >= _maxY ); }
    
    protected T[,] _objs = null;
    int _maxX, _maxY;
    public int MaxX { get { return _maxX; } }
    public int MaxY { get { return _maxY; } }
    public virtual T this[int x, int y] { get { return _objs[x, y]; } set { _objs[x, y] = value; } }
    
    /*
    public virtual T this[Pt pt] { get { return _objs[pt.x, pt.y, pt.z]; } set { _objs[pt.x, pt.y, pt.z] = value; } }

    public T Front(Pt pt) { return pt.z + 1 >= MaxZ ? null : this[pt.x, pt.y, pt.z + 1]; }
    public T Back(Pt pt) { return pt.z - 1 < 0 ? null : this[pt.x, pt.y, pt.z - 1]; }
    public T Right(Pt pt) { return pt.x + 1 >= MaxX ? null : this[pt.x + 1, pt.y, pt.z]; }
    public T Left(Pt pt) { return pt.x - 1 < 0 ? null : this[pt.x - 1, pt.y, pt.z]; }
    */
}
