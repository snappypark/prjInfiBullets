using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

public static class dCore
{
    /* void OnValidate()
    {string gi = "lineshot-stage-1.1.0";
    if (Application.isPlaying) ObscuredPrefs.CryptoKey = gi; }*/

    public static void Init()
    {
        dAds.Load();
        dSys.Load();
        dStage.Load();
#if UNITY_EDITOR
        dStage.lv = Mathf.Clamp(jsons.GetStartLvOfEdit(), 0, 99999);
#endif
    }

    public static void Save_PlayInfo()
    {
#if UNITY_EDITOR
#else
        ObscuredPrefs.SetInt("xdis", dStage.lv);
        ObscuredPrefs.Save();
#endif
    }
}

public static class dStage {
    public static scInt lv = new scInt();
    public static void Load() {      lv.SetValue(ObscuredPrefs.GetInt("xdis", 1)); }
    public static void AddStage() {  lv.SetValue(Mathf.Clamp(lv + 1, 1, jsons.LastStage()));  }
}

public static class dSys
{
    public static scBool music = new scBool();
    public static scInt sound = new scInt();
    public static scBool tuto = new scBool();
    
    public static void Load()
    {
        music.SetValue(ObscuredPrefs.GetBool("music", true));
        sound.SetValue(ObscuredPrefs.GetInt("sound", 1)); 
        tuto.SetValue(ObscuredPrefs.GetBool("tuto", true));
    }
    public static void Save()
    {
        ObscuredPrefs.SetBool("music", music);
        ObscuredPrefs.SetInt("sound", sound);
        ObscuredPrefs.Save();
    }
    public static void AfterTuto()
    {
        ObscuredPrefs.SetBool("tuto", false);
        ObscuredPrefs.Save();
    }
}

public static class dAds
{
    public static scBool NoAds = new scBool();
    static delay _delayReward = new delay(120);
    static delay _delayInterstitial = new delay(110);

    public static void Load()
    {
        NoAds.SetValue(ObscuredPrefs.GetBool("sdaon", false));
        _delayReward.Reset();
        _delayInterstitial.Reset();
    }

    public static void OnPurchase_NoAds()
    {
        NoAds = true;
        ObscuredPrefs.SetBool("sdaon", true);
        ObscuredPrefs.Save();
    }

/*
    public static void RequestAds()
    {
        if (Time.time > 90 && _delayInterstitial.IsEnd())
            ads.RequestInterstitial(false);
    }

    public static void ShowAds()
    {
        if (_delayReward.IsEnd() && !ads.HasReward())
        {
            ads.CreateAndLoadRewardedAd(false);
            _delayReward.Reset();
        }

        if (NoAds)
            return;

        if (!ads.HasBanner())
            ads.RequestBanner(false);

        if (_delayInterstitial.IsEnd())
        {
            if (ads.HasInterstitial())
                ads.ShowInterstitial();
            else
                uis.ads.ShowInterstitial();
            _delayInterstitial.Reset();
        }
    }
    */
}
