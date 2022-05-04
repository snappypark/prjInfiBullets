using System.Runtime.CompilerServices;

public struct idxArr
{
    public int idx;
    public int[] arr;
    public idxArr(int init, int max)
    {
        idx = init;
        arr = new int[max];
        for (int i = 0; i < max; ++i)
            arr[i] = i + 1;
        arr[max - 1] = 0;
    }
    public idxArr(int max)
    {
        idx = 0;
        arr = new int[max];
        for (int i = 0; i < max; ++i)
            arr[i] = i + 1;
        arr[max - 1] = 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetAfterRoll()
    {
        idx = arr[idx];
        return idx;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Roll()
    {
        idx = arr[idx];
    }
}

public class Idx
{
    public const int MaxNum2x2 = 4;
    public const int MaxNum3x3 = 9;
    public const int MaxNum5x5 = 25;
    public const int MaxNum7x7 = 49;

    static private int[] _IdxGapNN_I = new int[MaxNum7x7] { 0, //1
        -1,-1,0,  -1, 0,  1, 1, 1,                        // 9
        -2,-2,-2,-2,-2,  -1,-1,0,0,1,1,  2,2,2,2,2,         // 25
        -3,3,-3,3,-3,3,-3, -2,-2,-1,-1,0,0,1,1,2,2, -3,3,-3,3,-3,3,3
        };
    static private int[] _IdxGapNN_J = new int[MaxNum7x7] { 0,
        -1,0,-1,  1, 1,  -1, 0, 1,
        -2,-1,0,1,2,   -2,2,-2,2,-2,2,   -2, -1, 0, 1, 2,
        -3,-2,-1,0,1,2,3,  -3,3,-3,3,-3,3,-3,3,-3,3,  -2,-3,0,-1,2,1,3
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int INxN(int idx)
    {
        return _IdxGapNN_I[idx];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int JNxN(int idx)
    {
        return _IdxGapNN_J[idx];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Pt PtNxN(int idx)
    {
        return new Pt(_IdxGapNN_I[idx], _IdxGapNN_J[idx]);
    }

}
