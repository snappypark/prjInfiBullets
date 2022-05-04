using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiLvUp : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] RectTransform _rect;

    delay _scale = new delay(0.3f);
    delay _move = new delay(2.7f);
    public void Show()
    {
        endTime = Time.time + 3;
        _scale.Reset();
        _move.Reset();
        _rect.anchoredPosition = uis.WorldToCanvas(  I_I.Center+ new Vector3(0,1));
        transform.localScale = new Vector3(0.1f, 0.1f, 1);
        gameObject.SetActive(true);
    }

    public float endTime = 0;

    private void Update()
    {

        if (_move.InTime())
        {
            _rect.anchoredPosition += Vector2.up * Time.smoothDeltaTime * 11.2f;
        }
        else
        {
            gameObject.SetActive(false);
        }
        if (_scale.InTime())
        {
            float scale = 0.1f + _scale.Ratio01() * 1.0f;
            _rect.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void OnDisable()
    {
        _rect.anchoredPosition = Vector3.zero;
        _rect.localScale = Vector3.zero;
    }
}
