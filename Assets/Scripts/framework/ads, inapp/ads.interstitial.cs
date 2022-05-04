using System;
using GoogleMobileAds.Api;

public partial class ads
{   
    InterstitialAd _interstitial = null;

    public void LoadInterstitial(bool isTest = false)
    {
        if(dAds.NoAds)
            return;
        if(_interstitial == null)
        {
            //_interstitial = new InterstitialAd("ca-app-pub-6854154214780008/2051535072");
            _interstitial = new InterstitialAd("ca-app-pub-9839048061492395/5571143539");
            _interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
            _interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
            _interstitial.LoadAd(this.createAdRequest());
        }
    }

    public void ShowInterstitial()
    {
        if(dAds.NoAds)
            return;
        if(_interstitial != null && _interstitial.IsLoaded())
            _interstitial.Show();
        else
            uis.ads.ShowInterstitial();
    }

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args) {
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
        if (_interstitial != null)
            _interstitial.Destroy();
        _interstitial = null;
    }
    
    #endregion
}
//https://developers.google.com/admob/unity/interstitial?hl=ko