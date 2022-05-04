using UnityEngine;

public class endfield : MonoBehaviour
{
    public static int z;

    [SerializeField] TextMesh _lbStage;
    [SerializeField] ParticleSystem _dirGreen1;
    [SerializeField] ParticleSystem _dirGreen2;

    public void Init(int stageIdx)
    {
        _lbStage.text = langs.Stage(stageIdx+1, jsons.LastStage());
        transform.localPosition = new Vector3(6.5f,0,z+7);
        
        _dirGreen1.Stop();
        _dirGreen2.Stop();
    }

    public void Enter()
    {
        _dirGreen1.Play(true);
        _dirGreen2.Play(true);
        area.begin.Clear();
    }

}
