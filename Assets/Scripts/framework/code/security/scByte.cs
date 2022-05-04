using UnityEngine;

namespace nj
{ 
[System.Serializable]
public struct scByte
{
    [System.NonSerialized]
    static readonly int MAX_TSSDATA = 2;

    [System.NonSerialized]
    TssSdtByte[] m_tssData;
    
    bool IsSafe()
    {
        for (int i = 1; i < MAX_TSSDATA; i++)
        {
            int data1 = (byte)m_tssData[i];
            int data2 = (byte)m_tssData[i - 1];
            int dif = data1 - data2;
            if (dif != 0)
            {
                sc_static.ProblemDetected("[SecureByte] dif=" + dif + ", data1=" + data1 + ", data2=" + data2);
                return false;
            }
        }

        return true;
    }

    private byte GetValue()
    {
        if (m_tssData != null)
        {
            if (IsSafe())
                return (byte)m_tssData[0];
        }
        return 0;
    }
    private void SetValue(byte v)
    {
        if (m_tssData == null)
            m_tssData = new TssSdtByte[MAX_TSSDATA];

        for (int i = 0; i < MAX_TSSDATA; i++)
            m_tssData[i] = v;
    }

    public int CompareTo(scByte value)
    {
        int v0 = GetValue();
        int v1 = (byte)value;

        return (v0 - v1);
    }

    public static implicit operator byte(scByte v)
    {
        return v.GetValue();
    }

    public static implicit operator scByte(byte v)
    {
        scByte obj = new scByte();
        obj.SetValue(v);
        return obj;
    }

    public static scByte operator ++(scByte v)
    {
        byte value = v.GetValue();
        value++;
        v.SetValue(value);
        return v;
    }

    public static scByte operator --(scByte v)
    {
        byte value = v.GetValue();
        value--;
        v.SetValue(value);
        return v;
    }
    public override string ToString()
    {
        return string.Format("{0}", GetValue());
    }
}
}