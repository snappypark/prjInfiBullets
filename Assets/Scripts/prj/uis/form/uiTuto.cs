using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiTuto : MonoBehaviour
{
    [SerializeField] uiRatio _trFiger;
    [SerializeField] uiRatio _trLabel;
    [SerializeField] Text _lb;

    enum state{ init, left, onLeft, right, onRight, rotate, onRotate, none}
    state _state = state.init;

    delay _stateDly = new delay(3);
    delay _moveDly = new delay(3);
    delay _bounceDly = new delay(3);

    public void Show()
    {
        gameObject.SetActive(true);
        setImg(-0.85f, -0.25f);
        setState(state.init, 1, string.Empty);
    }
    
    void FixedUpdate()
    {
        switch(_state)
        {
            case state.init:
            if(_stateDly.IsEnd())
                setState(state.left, 0.7f, string.Empty);
            break;
            case state.left:
            setImg(-0.85f+_stateDly.Ratio01()*0.5f, -0.25f);
            if(_stateDly.IsEnd())
                setState(state.onLeft, 2.7f, "Move Left");
            break;
            case state.onLeft:
            setImg(-0.35f, -0.25f - _stateDly.Ratio01()*0.022f);
            if(_stateDly.IsEnd())
            {
                setImg(0.85f, -0.25f);
                setState(state.right,  0.7f, string.Empty);
            }
            break;

            case state.right:
            setImg(0.85f-_stateDly.Ratio01()*0.5f, -0.25f);
            if(_stateDly.IsEnd())
                setState(state.onRight, 2.1f, "Move Right");
            break;
            case state.onRight:
            setImg(0.35f, -0.25f - _stateDly.Ratio01()*0.022f);
            if(_stateDly.IsEnd())
            {
                setImg(0, -0.75f);
                setState(state.rotate,  0.7f, string.Empty);
            }
            break;

            case state.rotate:
            setImg(-0.05f, -0.76f + _stateDly.Ratio01()*0.3f);
            if(_stateDly.IsEnd())
                setState(state.onRotate, 2.5f, "Aim");
            break;
            case state.onRotate:
            setImg(-0.05f+_stateDly.Ratio01()*0.11f, -0.46f );
            if(_stateDly.IsEnd())
            {
                _state = state.none;
                gameObject.SetActive(false);
                dSys.AfterTuto();
            }
            break;
        }    
    }
    const float yRotation = -0.35f;
    void setState(state state_, float nextduration, string label)
    {
        _state = state_;
        _stateDly.Reset(nextduration);
        _lb.text = label;
    }

    void setImg(float rx, float ry)
    {
        _trFiger.Active(rx, ry);
        _trLabel.Active(rx, ry+0.03f);
    }
}
