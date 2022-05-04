using UnityEngine;

using Beebyte.Obfuscator;

public class uiBtnAdLvUp : MonoBehaviour
{
    [SerializeField] RectTransform _rtMove;
    public void Active(bool show)
    {
        gameObject.SetActive(show);
    }

    toNfro _move = new toNfro(5.1f, 1.7f);
    private void Update()
    {
        _rtMove.anchoredPosition = new Vector2(-47, 58 + _move.GetDt());
    }

    [SkipRename]
    public void OnBtn_Ad_LvUp()
    {
     //   ads.ShowRewardedAd();
#if UNITY_EDITOR
#else
#endif
    }
}
