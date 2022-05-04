using UnityEngine;

public class beginfield : MonoBehaviour
{
    [SerializeField] TextMesh _lbStage;
    public void Init(int stageIdx)
    {
        _lbStage.text = langs.Stage(stageIdx, jsons.LastStage());
    }

    public void Clear()
    {
        _lbStage.text = string.Empty;
    }
}
