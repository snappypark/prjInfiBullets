using UnityEngine;

public partial class hero : MonoBehaviour
{
    Type _type; public enum Type { None, Play, Goal, Edit, Begin, End }
    [HideInInspector] public absHeroEntity entity;

    static hero _inst;
    public static i2 pt, pt11;
    [SerializeField] Transform _tr;
    [SerializeField] heroSpriter _spriter;

    [SerializeField] ParticleSystem _fxUp;
    [SerializeField] ParticleSystem _fxDown;
    [SerializeField] ParticleSystem _fxFire;
    
    void Awake()
    {
        _inst = this;
    }
    
    public static void Init(Type type)
    {        
        if (_inst.entity != null)
            GameObject.DestroyImmediate(_inst.entity);
        _inst._type = type;
        switch (type)  {
            case Type.Play: _inst.entity = _inst.gameObject.AddComponent<heroPlayEntity>(); break;
            case Type.Goal: _inst.entity = _inst.gameObject.AddComponent<heroGoalEntity>(); break;
            case Type.Edit: _inst.entity = _inst.gameObject.AddComponent<heroEditEntity>(); break;
            case Type.Begin: _inst.entity = _inst.gameObject.AddComponent<heroBeginEnity>(); break;
            case Type.End: _inst.entity = _inst.gameObject.AddComponent<heroEndEntity>(); break; 
            }
    }

    public static void UpdateTran()
    {
        pt = new i2((int)_inst._tr.localPosition.x, (int)_inst._tr.localPosition.z);
        backZ = pt.z - 4;
        pt11 = cell.Pt11(_inst._tr.localPosition);
        cam.OnUpdateByHero(_inst._tr);
    }

    public static void SetDirIdx(int idx) { _inst._spriter.SetDirIdx(idx); }
    public static void ResetPos(){ _inst._tr.localPosition = new Vector3(6.5f, _inst._tr.localPosition.y, 6.5f); }
    
    public static void MoveZ(float dt) { 
        _inst._tr.transform.localPosition += new Vector3(0,0,dt*Time.smoothDeltaTime);}
    public static void MoveLeft(float dt) {
        if(_inst._tr.transform.localPosition.x > 1.5f)
            _inst._tr.transform.localPosition -= new Vector3(dt*Time.smoothDeltaTime,0);}
    public static void MoveRight(float dt) {
        if(_inst._tr.transform.localPosition.x < 11.5f)
            _inst._tr.transform.localPosition += new Vector3(dt*Time.smoothDeltaTime,0);}
    
    public static void MoveCenter(float dt)
    {
        if(_inst._tr.transform.localPosition.x < 6.0f)
            _inst._tr.transform.localPosition += new Vector3(dt*Time.smoothDeltaTime,0);
        else if(_inst._tr.transform.localPosition.x > 7.0f)
            _inst._tr.transform.localPosition -= new Vector3(dt*Time.smoothDeltaTime,0);
    }

    public static void FxFire()
    {
        _inst._fxFire.Play();
    }

    public static void ShowSprite(bool value)
    {
        _inst._spriter.gameObject.SetActive(value);
    }

    public static void UpdateSpriteIdx()
    {
        _inst._spriter.UpdateIdx();
    }
}
