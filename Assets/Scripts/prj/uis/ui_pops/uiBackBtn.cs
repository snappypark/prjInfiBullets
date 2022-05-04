using UnityEngine;
using UnityEngine.UI;

public class uiBackBtn : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] Text _lbYes;
    [SerializeField] Text _lbNo;
    
    void OnEnable()
    {
        Time.timeScale = 0;
    }
    
    public void Show()
    {
        _text.text = langs.ExitGame();
        _lbYes.text = langs.Yes();
        _lbNo.text = langs.No();
        gameObject.SetActive(true);
    }

    #region ui action
    public void OnBtn_OK()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void OnBtn_NO()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    #endregion
}
