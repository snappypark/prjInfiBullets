using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_pops : MonoBehaviour
{
    [SerializeField] ui_popOption _option;
    [SerializeField] ui_popup _popup;
    [SerializeField] public uiBackBtn BackBtn;

    public void ShowOption()
    {
        _option.gameObject.SetActive(true);
    }
    
    public void Show(string str)
    {
        _popup.Show(str);
    }
    
}
