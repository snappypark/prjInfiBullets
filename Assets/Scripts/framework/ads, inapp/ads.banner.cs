using System;
using GoogleMobileAds.Api;

public partial class ads
{
    BannerView _banner;
    bool _isLoaded = false;
    
    bool _isShown = false;
    public void LoadBanner()
    {
        if(dAds.NoAds)
            return;
        if (_banner != null)
           return;
        _isShown = true;
        //_banner = new BannerView("ca-app-pub-6854154214780008/1754993934", AdSize.Banner, AdPosition.Top);
        _banner = new BannerView("ca-app-pub-9839048061492395/6884225203", AdSize.Banner, AdPosition.Top);
        _banner.OnAdLoaded += this.HandleAdLoaded;
        _banner.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        _banner.LoadAd(createAdRequest());
    }

    public void ShowBanner()
    {
        if(dAds.NoAds)
            return;
        if(_isShown)
            return;
        _isShown = true;
        
        if (_banner != null && _isLoaded)
        {
            _banner.Show();
            uis.ads.ShowBanner(false);
        }
        else
            uis.ads.ShowBanner(true);
    }
    
    public void HideBanner()
    {
        if(!_isShown)
            return;
        _isShown = false;

        if (_banner != null && _isLoaded)
            _banner.Hide();
        uis.ads.ShowBanner(false);
    }

    public void HandleAdLoaded(object s, EventArgs a) {
        _isLoaded = true;
        uis.ads.ShowBanner(false);
    }

    public void HandleAdFailedToLoad(object s, AdFailedToLoadEventArgs a) {
        destoryBanner();
        uis.ads.ShowBanner(true);
    }

    void destoryBanner()
    {
        if (_banner != null)
            _banner.Destroy();
        uis.ads.ShowBanner(false);
        _banner = null;
        _isLoaded = false;
    }
}
