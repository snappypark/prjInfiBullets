using UnityEngine;
using UnityEngine.UI;

using Beebyte.Obfuscator;

public class ui_popup : MonoBehaviour
{
    [SerializeField] Text _text;
    public void Show(string context)
    {
        _text.text = context;
        gameObject.SetActive(true);
    }

    #region UI ACTION

    [SkipRename]
    public void OnBtn_OK()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
