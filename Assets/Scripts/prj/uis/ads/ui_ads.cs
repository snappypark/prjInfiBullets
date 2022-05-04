using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Beebyte.Obfuscator;

public class ui_ads : MonoBehaviour
{
    [SerializeField] GameObject _Banner;
    [SerializeField] GameObject _Interstitial;

    public void ShowBanner(bool value)
    {
        //_Banner.SetActive(value);
    }

    public void NotShowBanner()
    {
        _Banner.SetActive(false);
    }

    public void ShowInterstitial()
    {
        Time.timeScale = 0;
        _Interstitial.SetActive(true);
    }


    #region UI ACTION

    [SkipRename]
    public void OnBtn_Banner()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ninetyjay.MazeZombieBreak");
    }

    [SkipRename]
    public void OnBtn_Interstitial()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ninetyjay.MazeZombieBreak");
    }
    [SkipRename]
    public void OnBtn_CloseInterstitial()
    {
        Time.timeScale = 1;
        _Interstitial.SetActive(false);
    }
    #endregion
}
