using UnityEngine;
using UnityEngineEx;
using UnityEngine.UI;

public partial class ui_form : MonoBehaviour
{
    [SerializeField] public uiTuto tuto;
    [SerializeField] AniCurveEx_TranScale _trStage;
    [SerializeField] Text _textStage;

    [SerializeField] Slider _slider;
    bool _isOnLeftBtn = false, _isOnRightBtn = false;
    public bool IsOn_BtnLeft{get{return _isOnLeftBtn;}}
    public bool IsOn_BtnRight{get{return _isOnRightBtn;}}

    public void OnActive()
    {
        gameObject.SetActive(true);
        if(dSys.tuto)
            tuto.Show();
    }
    
    public void RefreshStage()
    {
        _textStage.text = langs.StageSharp(dStage.lv, jsons.LastStage());
    }
    
    public void RefreshSlider(int idx)
    {
        _slider.value = idx;
    }

    #region UI Action
    public void onBtn_option()
    {
        if (uis.IsWaitingBtn())
            return;
        uis.pops.ShowOption();
    }

    public void OnBtn_Dir(){ hero.SetDirIdx((int)_slider.value); }
    public void OnBtnPress_Left(){   _isOnLeftBtn = true; }
    public void OnBtnRelease_Left(){ _isOnLeftBtn = false; }
    public void OnBtnPress_Right(){  _isOnRightBtn = true; }
    public void OnBtnRelease_Right(){_isOnRightBtn = false; }
    #endregion
}
