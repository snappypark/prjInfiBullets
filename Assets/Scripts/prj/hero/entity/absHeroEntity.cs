using UnityEngine;

public abstract class absHeroEntity : MonoBehaviour
{
    protected void resetDirection()
    {
        hero.SetDirIdx(13);
        uis.form.RefreshSlider(13);
    }
}
