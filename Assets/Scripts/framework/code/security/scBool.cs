[System.Serializable]
public struct scBool
{
    [System.NonSerialized]
    static readonly int MAX_TSSDATA = 2;

    [System.NonSerialized]
    TssSdtByte[] m_tssData;

    /*
#if UNITY_EDITOR
    [SerializeField]
    bool data;
#endif
*/

    bool IsSafe()
    {
        for (int i = 1; i < MAX_TSSDATA; i++)
        {
            byte data1 = (byte)m_tssData[i];
            byte data2 = (byte)m_tssData[i - 1];
            byte dif = (byte)(data1 - data2);
            if (dif != 0)
            {
                sc_static.ProblemDetected("[SecureBool] dif=" + dif + ", data1=" + data1 + ", data2=" + data2);
                return false;
            }
        }

        return true;
    }

    public bool GetValue()
    {
        if (m_tssData != null)
        {
            if (IsSafe())
                return (m_tssData[0] != 0);
        }
        return false;
    }

    public void SetValue(bool v)
    {
        if (m_tssData == null)
            m_tssData = new TssSdtByte[MAX_TSSDATA];

        for (int i = 0; i < MAX_TSSDATA; i++)
            m_tssData[i] = (byte)(v ? 1 : 0);
        /*
#if UNITY_EDITOR
        data = v;
#endif*/
    }

    public int CompareTo(scBool value)
    {
        int v0 = GetValue() ? 1 : 0;
        int v1 = (bool)value ? 1 : 0;

        return (v0 - v1);
    }

    public static implicit operator bool(scBool v)
    {
        return v.GetValue();
    }

    public static implicit operator scBool(bool v)
    {
        scBool obj = new scBool();
        obj.SetValue(v);
        return obj;
    }

    public override string ToString()
    {
        return string.Format("{0}", GetValue() ? "true" : "false");
    }
}
