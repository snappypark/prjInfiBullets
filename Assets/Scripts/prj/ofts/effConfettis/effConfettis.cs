using System.Collections;
using System.Collections.Generic;
using System.Collections.GenericEx;
using UnityEngine;

public class effConfettis : MonoBehaviour
{
    static effConfettis _inst;

    [SerializeField] ParticleSystem[] _pss;
    [SerializeField] ParticleSystem _dirGreen;

    List<int> _idxs = new List<int>();
    int idx = 0;

    void Awake()
    {
        _inst = this;
        for(int i=0; i<6; ++i)
            _idxs.Add(i);
        _idxs.Shuffle(0, _idxs.Count);
    }

    public static void PlayDirGreen()
    {
        _inst._dirGreen.Play();
    }

    public static void Fire(int z)
    {
        if(_inst.idx > 5)
            return;
        _inst._pss[_inst.idx].gameObject.transform.localPosition = new Vector3(
            Random.Range(1, 15), 
            Random.Range(6, 9.0f), 
            z + Random.Range(2.5f, 9.0f));
        _inst._pss[_inst.idx].Play();
        ++_inst.idx;
    }

    public static void Shuffle()
    {
        _inst._idxs.Shuffle(0, _inst._idxs.Count);
        _inst.idx = 0;
    }

}
