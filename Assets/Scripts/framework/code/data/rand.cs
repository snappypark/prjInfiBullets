using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class rand
{
    static uint seed = 61829450;
    public static float gauss(int gap) // gap  3
    {
        float sum = 0;
        for (int i = 0; i < gap; i++)
        {   //Uses an xorshift PRNG
            uint hold = seed;
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            int r = (int)(hold + seed);
            sum += (float)r * (1.0f / 0x7FFFFFFF);
        }
        //Returns [-3.0,3.0] (66.7%–95.8%–100%)
        return sum;
    }

}
