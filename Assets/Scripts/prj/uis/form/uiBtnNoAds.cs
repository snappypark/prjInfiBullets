using UnityEngine;
using UnityEngineEx;
using UnityEngine.UI;
using Beebyte.Obfuscator;

public class uiBtnNoAds : MonoBehaviour
{
    [SerializeField] RectTransform _rt;

    public void Refresh()
    {
        bool active = InApp.IsInitialized() && !dAds.NoAds;
        _rt.anchoredPosition = new Vector2(active ? -73 : 200, -60);
    }

    [SkipRename]
    public void OnBtn_Buy()
    {
        if (uis.IsWaitingBtn(2.7f))
            return;
        InApp.BuyProductID();
    }
}
