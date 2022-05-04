using UnityEngine;
using UnityEngine.UI;

public class ui_popOption : MonoBehaviour
{
    [SerializeField] Sprite[] _imgMusics;
    [SerializeField] Sprite[] _imgSounds;
    [SerializeField] Image _imgMusic;
    [SerializeField] Image _imgSound;
    [SerializeField] Text _lbRate;
    [SerializeField] Text _lbRecommand;

    void OnEnable()
    {
        Time.timeScale = 0.0f;
        _imgMusic.sprite = dSys.music ? _imgMusics[0] :  _imgMusics[1];
        _imgSound.sprite = _imgSounds[dSys.sound];
        _lbRate.text = langs.rateUs();
        _lbRecommand.text = langs.tryNow();
    }

    void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

    #region Action
    
    public void onBtn_music()
    {
        if(dSys.music)
        {
            AppAudio.Inst.StopMusic();
            dSys.music.SetValue(false);
        }
        else 
        {
            AppAudio.Inst.PlayMusic(AppAudio.eMusicType.ingame);
            dSys.music.SetValue(true);
        }
        _imgMusic.sprite = dSys.music ? _imgMusics[0] :  _imgMusics[1];
        dSys.Save();
    }

    int tmp = 10;
    public void onBtn_sound()
    {
        /*
        //*/
        if(tmp < 0)
        {
            dStage.AddStage();
            hero.Init(hero.Type.End);
        }
        else
            --tmp;

        if(dSys.sound == 1)
            dSys.sound.SetValue(0);
        else
            dSys.sound.SetValue(dSys.sound+1);
        int idx = dSys.sound;
        if(idx == 0)
            AppAudio.Inst.Mute();
        else
            AppAudio.Inst.UnMute();
        _imgSound.sprite = _imgSounds[idx];
        dSys.Save();
    }

    public void onBtn_close()
    {
        gameObject.SetActive(false);
    }
    
    public void onBtn_rate()
    { 
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ninetyjay.BouncyBullets");
 
    }
    public void onBtn_recommand()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.ninetyjay.MazeZombieBreak");
    }
    #endregion
}
