using UnityEngine;

[ExecuteInEditMode]
public class uiRatio : MonoBehaviour
{
    enum ePosType { Default, ByWidth}
    enum eSizeType { Default, ByHeight, ByWidth }

    [SerializeField] RectTransform _rt;
    [SerializeField] ePosType _posType = ePosType.Default;
    [SerializeField] eSizeType _sizeType = eSizeType.Default;
    [SerializeField] Vector2 _posRatio;
    [SerializeField] Vector2 _sizeRatio;
    
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
        _posRatio = new Vector2(xPosRatio_, yPosRatio_);
        _sizeRatio = new Vector2(xSzRatio_, ySzRatio_);
        
        switch (_posType)
        {
            case ePosType.ByWidth:
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, screenWidth * yPosRatio_);
                break;
            default:
                _rt.anchoredPosition = new Vector2(screenWidth * xPosRatio_, screenHeight * yPosRatio_);
                break;
        }
        switch (_sizeType)
        {
            case eSizeType.ByHeight:
                _rt.sizeDelta = new Vector2(screenHeight * xSzRatio_, screenHeight * ySzRatio_);
                break;
            case eSizeType.ByWidth:
                _rt.sizeDelta = new Vector2(screenWidth * xSzRatio_, screenWidth * ySzRatio_);
                break;
            default:
                _rt.sizeDelta = new Vector2(screenWidth * xSzRatio_, screenHeight * ySzRatio_);
                break;
        }
    }
}
