using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngineEx;

using Beebyte.Obfuscator;

public partial class ui_form
{
    [System.Serializable]
    class joystick
    {
        [SerializeField] public ScrollRect Scroll = null;
        [SerializeField] public RectTransform TransBtn = null;
        [HideInInspector] public float sqrDist = 0.0f;
        [HideInInspector] public Vector2 Dir = Vector2.zero;
    }

    [SerializeField] joystick _joystick;
    float _sqrRange = 325.0f;

    public bool IsMoveJoystic = false;
    
    void onUpdate_Joystick()
    {
#if UNITY_EDITOR
        _joystick.Scroll.gameObject.SetActive(!flowEdit.IsOn);
#endif

        IsMoveJoystic = _joystick.TransBtn.anchoredPosition.sqrMagnitude > _sqrRange;
        switch (IsMoveJoystic)
        {
            case true:
        //        shooter.Aiming(new Vector3(-_joystick.TransBtn.anchoredPosition.x, -_joystick.TransBtn.anchoredPosition.y, 0).normalized);
                break;
            case false:
         //       shooter.UnAiming();
                break;
        }

    }

    #region UI Action
    [SkipRename]
    public void OnScroll_JoystickMove(Vector2 value)
    {
        /*
        if (_joystick.sqrDist < _sqrRange)
            return;
        switch (App.Inst.FlowMgr.CurType)
        {
            case ProjS.Flow.iTypeMenu:
                break;
            case ProjS.Flow.iTypePlay:
              //  _joystickAct.TransBtn.anchoredPosition = _joystickAct.Dir * _JoystickRange.y;
                break;
        }*/
    }

    #endregion
}
