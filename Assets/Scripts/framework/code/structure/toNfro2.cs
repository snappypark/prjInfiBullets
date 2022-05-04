using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct toNfro2
{
    enum eState
    {
        back,
        forth,
    }

    eState _state;
    float _speed;
    float _gap;
    float _dt;
    toNfro _tf;

    public toNfro2(float speed_, float gap_)
    {
        _state = eState.forth;
        _speed = speed_;
        _gap = gap_;
        _dt = 0.0f;
        _tf = new toNfro(26, 7.93f);
    }

    public float GetDt()
    {
        _speed = 8.0f + _tf.GetDt();
        switch (_state)
        {
            case eState.back:
                _dt -= Time.smoothDeltaTime * _speed;
                if (_dt < -_gap)
                    _state = eState.forth;
                break;
            case eState.forth:
                _dt += Time.smoothDeltaTime * _speed;
                if (_dt > _gap)
                    _state = eState.back;
                break;
        }
        return _dt;
    }
}
