using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx;
using UnityEngine.UI;

public class uiPlayCnt : MonoBehaviour
{
    [SerializeField] AniCurveEx_TranScale _tapTextScale;
    [SerializeField] RectTransform _rtTap;
    [SerializeField] Text _lbText;

    public void Active(int cnt)
    {
        _tapTextScale.Update(2.0f);
        _rtTap.anchoredPosition =
            uis.WorldToCanvas(I_I.Center + new Vector3(0, 2.7f));


        _lbText.text = string.Format("{0}", cnt);
        if (gameObject.activeSelf)
            return;
        _lbText.text = string.Empty;
        gameObject.SetActive(true);
    }
    
    public void Inactive()
    {
        if (!gameObject.activeSelf)
            return;
        _lbText.text = string.Empty;
        gameObject.SetActive(false);
    }

}
