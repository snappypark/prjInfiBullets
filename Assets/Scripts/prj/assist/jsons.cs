using System;
using System.Text;
using UnityEngine;

public class jsons : MonoBehaviour
{
    static jsons _inst;
    public static int GetStartLvOfEdit(){ return _inst._startlvOfEdit; }
    [SerializeField] int _startlvOfEdit = 1;
    [SerializeField] txts_ _stages;
    [SerializeField] TextAsset _prj;
    public static int LastStage(){ return _inst._stages.Num-1;}
    public static int NumStage(){ return _inst._stages.Num;}

    void Awake()
    {
        _inst = this;
    }

    public static string GetPrjJson()
    {
        string result = Encoding.UTF8.GetString(_inst._prj.bytes);
        byte[] decodedBytes = Convert.FromBase64String(result);
        return Encoding.UTF8.GetString(decodedBytes);
    }

    public static string GetStageJson(int idx) { return _inst._stages.GetJson(idx); }
    public static string GetStageName(int idx) { return _inst._stages[idx].name; }

    [System.Serializable]
    private class txts_
    {
        [SerializeField] TextAsset[] textAssets;
        public int Num { get { return textAssets.Length; } }

        public TextAsset this[int idx] { get { return textAssets[idx]; } }

        public string GetJson(int idx)
        {
            string result = Encoding.UTF8.GetString(textAssets[idx].bytes);
            byte[] decodedBytes = Convert.FromBase64String(result);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}
