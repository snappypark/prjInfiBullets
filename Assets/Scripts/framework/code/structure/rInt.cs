using UnityEngine;

public struct rInt
{
    public int value;
    public int max;
    public float maxOver;

    public rInt(int valueWithMax_)
    {
        value = valueWithMax_;
        max = valueWithMax_;
        maxOver = (1 / (float)valueWithMax_);
    }
    public rInt(int value_, int max_)
    {
        value = value_;
        max = max_;
        maxOver = (1 / (float)max_);
    }

    public float Ratio01()
    {
        return Mathf.Clamp01(value*maxOver);
    }

    public bool isZero()
    {
        return value == 0;
    }

    public bool isFull()
    {
        return value == max;
    }

    public bool isNotFull()
    {
        return value < max;
    }

    public void increase(int inc = 1)
    {
        value += inc;
    }
    
    public void decrease(int dec = 1)
    {
        value -= dec;
    }

    public void increaseClamp(int inc = 1)
    {
        value = Mathf.Clamp(value + inc, 0, max);
    }

    public void decreaseClamp(int dec = 1)
    {
        value = Mathf.Clamp(value - dec, 0, max);
    }

    public static implicit operator int(rInt r)
    {
        return r.value;
    }
}
