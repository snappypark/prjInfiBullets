using System;
using GoogleMobileAds.Api;

public partial class ads
{
    RewardedAd _reward10 = null;
    
    public void LoadRewardX10(bool isTest = false)
    {
        if(_reward1 != null)
            return;
        _reward10 = new RewardedAd(isTest ? _testReawrd10Id : "ca-app-pub-9839048061492395/2972439629");

        _reward10.OnAdLoaded += HandleRewardedAdLoaded;
        _reward10.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        _reward10.OnAdClosed += HandleRewardedAdClosed;
        _reward10.OnUserEarnedReward += HandleUserEarnedReward;
        
        _reward10.LoadAd(createAdRequest());
    }
    
    public bool IsLoadRewardX10()
    {
        return _reward10 != null && _reward10.IsLoaded();
    }

    public void ShowRewardX10()
    {
        /*
        if (_reward10 != null && _reward10.IsLoaded())
        {
            if(dCust.HasAll())
                uis.pops.Show_Ok(langs.fullOfItems());
            else
                _reward10.Show();
            _reward1 = null;
        }
        else
            uis.pops.Show_Ok(langs.adIsNotReady());*/
    }
    
    public void HandleUserEarnedReward(object sender, Reward args) {
        _reward10 = null;
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args) {
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) {
        _reward10 = null;
    }
    
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
        _reward10 = null;
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args) {
        _reward10 = null;
    }
}
