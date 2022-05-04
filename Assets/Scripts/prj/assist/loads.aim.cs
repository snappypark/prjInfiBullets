using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngineExJSON;

public static partial class loads
{
    const int _numSheet = 26;
    public static f2[,] fxGaps = new f2[_numSheet, 30];
    public static f2[,] aimGaps = new f2[_numSheet, 30];
    public static f2[,] aimDirs = new f2[_numSheet, 30];

    public static IEnumerator PrjFromJson_()
    {
        JSONObject jsPrj = new JSONObject(jsons.GetPrjJson())[jsK.Proj];
        jsK.ObjAimGaps = jsPrj[jsK.AimGaps];
        yield return null;
        for (int i = 0; i < jsK.ObjAimGaps.Count; ++i)
        {
            jsArr arJs = new jsArr(jsK.ObjAimGaps[i]);
            int idxDir = arJs.Int;
            int idx = arJs.Int;
            aimGaps[idxDir, idx] = new f2(arJs.F, arJs.F);
            aimDirs[idxDir, idx] = new f2(arJs.F, arJs.F);
            fxGaps[idxDir, idx] = aimGaps[idxDir, idx];
        }
        
        yield return null;
        f2[] sumDirs = new f2[_numSheet];
        for (int i = 0; i < _numSheet; ++i)
        {
            sumDirs[i] = new f2();
            for (int j = 0; j < 30; ++j)
            {
                sumDirs[i] += aimDirs[i, j];
            }
            sumDirs[i].Normalize();
            for (int j = 0; j < 30; ++j)
            {
                aimDirs[i, j] = (aimDirs[i, j] + sumDirs[i] * 1.27f);
                aimDirs[i, j].Normalize();
                aimGaps[i, j] += new f2(0.015f,0.311f) + aimDirs[i, j]*0.62f;
                fxGaps[i, j] += new f2(0.015f,0.311f) + aimDirs[i, j]*0.32f;
            }
        }
        
        yield return null;
    }

}
