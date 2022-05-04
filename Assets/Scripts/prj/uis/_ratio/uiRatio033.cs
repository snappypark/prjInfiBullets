using UnityEngine;

[ExecuteInEditMode]
public class uiRatio033 : MonoBehaviour
{
    enum ePosType { Upper033, UpperChild033, Lower, LowerChild033}
    enum eSizeType { Upper033, Lower, LowerChild033 }

    [SerializeField] RectTransform _rt;
    [SerializeField] ePosType _posType = ePosType.Upper033;
    [SerializeField] eSizeType _sizeType = eSizeType.Upper033;
    [SerializeField] Vector2 _posRatio, _sizeRatio;
    
    void OnEnable()
    {
        Refresh(_posRatio.x, _posRatio.y, _sizeRatio.x, _sizeRatio.y);
    }
    
    public void Inactive()
    {
        gameObject.SetActive(false);
    }

    public void Active(float xPosRatio_, float yPosRatio_)
    {
        gameObject.SetActive(true);
        Refresh(xPosRatio_, yPosRatio_, _sizeRatio.x, _sizeRatio.y);
    }

    public void Refresh(float xPosRatio_, float yPosRatio_, float xSzRatio_, float ySzRatio_)
    {
        if (uis.IsNullInst)
            return;
        float screenWidth = uis.Width;
        float screenHeight = uis.Height;
        float gameHeight = screenWidth*1.5f;
        _posRatio = new Vector2(xPosRatio_, yPosRatio_);
        _sizeRatio = new Vector2(xSzRatio_, ySzRatio_);
        
        switch (_posType) {
            case ePosType.Upper033:
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, screenHeight*0.5f - gameHeight*yPosRatio_);
                break;
            case ePosType.UpperChild033:
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, - gameHeight*yPosRatio_);
                break;
            case ePosType.Lower:
                float upper = gameHeight * 0.032f;
                float lower = screenHeight - (gameHeight+upper);
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, -screenHeight*0.5f + lower*0.5f);
                break;
            case ePosType.LowerChild033:
                float upper2 = gameHeight * 0.032f;
                float lower2 = screenHeight - (gameHeight+upper2);
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, lower2*0.5f);
                break;
        }
        switch (_sizeType) {
            case eSizeType.Upper033:
                _rt.sizeDelta = new Vector2(screenWidth * xSzRatio_, gameHeight * ySzRatio_);
                break;
            case eSizeType.Lower:
                float upper = gameHeight * 0.032f;
                float lower = Mathf.Max(0, screenHeight - (gameHeight+upper));
                _rt.sizeDelta = new Vector2(screenWidth * xSzRatio_, lower);
                break;
            case eSizeType.LowerChild033:
                float upper2 = gameHeight * 0.032f;
                float lower2= Mathf.Max(0, screenHeight - (gameHeight+upper2));
                _rt.sizeDelta = new Vector2(screenWidth * xSzRatio_, lower2* ySzRatio_);
                break;

                
        }
    }
}
