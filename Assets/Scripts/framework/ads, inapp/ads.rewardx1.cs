using System;
using GoogleMobileAds.Api;

public partial class ads
{
    RewardedInterstitialAd _reward1 = null;

    public void LoadRewardX1(bool isTest = false)
    {
        if(_reward1 == null)
            RewardedInterstitialAd.LoadAd("ca-app-pub-9839048061492395/3002844035", createAdRequest(), 
                                            (RewardedInterstitialAd ad, string error)=>{
                                                if (error == null)
                                                {
                                                    _reward1 = ad;
                                                    _reward1.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresent;
                                                    _reward1.OnAdDidPresentFullScreenContent += HandleAdDidPresent;
                                                    _reward1.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
                                                    _reward1.OnPaidEvent += HandlePaidEvent;
                                                }
                                                else 
                                                    _reward1 = null;
                                            });
    }

    public bool IsLoadRewardX1()
    {
        return _reward1 != null;
    }

    public void ShowRewardX1()
    {
        if(_reward1 != null)
        {
            _reward1.Show((Reward reward)=>{ 
                
                } );
            _reward1 = null;
        }
        else
            {}//uis.pops.Show_Ok(langs.adIsNotReady());
    }
        
    private void HandleAdFailedToPresent(object sender, AdErrorEventArgs args)
    {
        _reward1 = null;
    }

    private void HandleAdDidPresent(object sender, EventArgs args)
    {
        _reward1 = null;
    }

    private void HandleAdDidDismiss(object sender, EventArgs args)
    {
        _reward1 = null;
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        _reward1 = null;
    }
}
