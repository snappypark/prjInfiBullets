using UnityEngine;

public class heroSpriter : MonoBehaviour
{
    [System.Serializable]
    class sprites { [SerializeField] public Sprite[] _sprits; }
    
    [SerializeField] SpriteRenderer _render;
    [SerializeField] sprites[] _sheet;
    int _dirIdx = 12;
    int _aniIdx = 0;

    public void SetDirIdx(int idx) { _dirIdx = idx; }
    public int GetDirIdx() { return _dirIdx; }
    public int GetAniIdx() { return _aniIdx; }

    static delay _delay = new delay(1.1f);
    
    void Update()
    {
        ++_aniIdx;
        if (_aniIdx > 29)
            _aniIdx = 0;
    }

    void FixedUpdate()
    {
       // if (_delay.InTime())
       //     return;
      //  _delay.Reset();
        _render.sprite = _sheet[_dirIdx]._sprits[_aniIdx];
    }

    public void SetColor(Color color)
    {
        _render.color = color;
    }

    public void UpdateIdx()
    {
        ++_aniIdx;
        if (_aniIdx > 29)
            _aniIdx = 0;
    }
}

