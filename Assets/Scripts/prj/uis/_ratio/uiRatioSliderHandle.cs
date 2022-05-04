using UnityEngine;

public class uiRatioSliderHandle : MonoBehaviour
{
    [SerializeField] RectTransform _rt;
    [SerializeField] float _xRatio;
    
    void OnEnable()
    {
        Refresh();
    }
    
    public void Refresh()
    {
        if (uis.IsNullInst)
            return;
        float screenWidth = uis.Width;
        float screenHeight = uis.Height;
        _rt.sizeDelta = new Vector2(screenWidth * _xRatio, 0);
        
    }
}
