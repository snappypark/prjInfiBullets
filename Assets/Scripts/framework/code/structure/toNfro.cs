using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct toNfro
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

    public toNfro(float speed_, float gap_, float dt_ = 0)
    {
        _state = eState.forth;
        _speed = speed_;
        _gap = gap_;
        _dt = dt_;
    }

    public float GetDt()
    {
        switch (_state)
        {
            case eState.back:
                _dt -= Time.fixedDeltaTime* _speed;
                if (_dt < -_gap)
                    _state = eState.forth;
                break;
            case eState.forth:
                _dt += Time.fixedDeltaTime * _speed;
                if (_dt > _gap)
                    _state = eState.back;
                break;
        }
        return _dt;
    }
}

