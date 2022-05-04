using UnityEngine;

public struct iM
{
    public int value;
    public int max;
    public float maxOver;

    public iM(bool zero)
    {
        value = 0;
        max = 0;
        maxOver = 0;
    }
    public iM(int max_)
    {
        value = max_;
        max = max_;
        maxOver = (1 / (float)max_);
    }
    public iM(int value_, int max_)
    {
        value = value_;
        max = max_;
        maxOver = (1 / (float)max_);
    }

    public void ResetMax(int max_)
    {
        max = max_;
        value = max;
        maxOver = (1 / (float)max_);
    }

    public void ResetMax()
    {
        value = max;
    }

    public float Ratio01()
    {
        return Mathf.Clamp01(value*maxOver);
    }

    public void Set(int v)
    {
        value = Mathf.Clamp(v, 0, max);
    }

    public void SetZero()
    {
        value = 0;
    }

    public bool isZero()
    {
        return value == 0;
    }

    public bool isNotZero()
    {
        return value != 0;
    }

    public bool isFull()
    {
        return value == max;
    }

    public bool isNotFull()
    {
        return value < max;
    }

    public void inc(int inc = 1)
    {
        value += inc;
    }
    
    public void dec(int dec = 1)
    {
        value -= dec;
    }

    public void incClamp(int inc = 1)
    {
        value = Mathf.Clamp(value + inc, 0, max);
    }

    public void decClamp(int dec = 1)
    {
        value = Mathf.Clamp(value - dec, 0, max);
    }

    public static implicit operator int(iM r)
    {
        return r.value;
    }

    public bool decreaseClamp_checkFirstZero(int dec = 1)
    {
        int original = value;
        value = Mathf.Clamp(value - dec, 0, max);
        return original > 0 && value == 0;
    }

    public bool increaseClamp_checkFirstZero(int inc = 1)
    {
        int original = value;
        value = Mathf.Clamp(value + inc, 0, max);
        return original > 0 && value == 0;
    }
}
