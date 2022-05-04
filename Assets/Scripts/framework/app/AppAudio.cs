using UnityEngine;

public class AppAudio : MonoSingleton<AppAudio>
{
    static AppAudio _inst;
    public enum eMusicType { ingame, }
    public enum eSoundType { hit = 0, }

    [SerializeField] AudioClip[] _musics;
    [SerializeField] AudioSource _musicSource;

    [SerializeField] AudioClip[] _sounds;
    [SerializeField] AudioSource[] _soundSources;

    [SerializeField] AudioSource _mainEffect;
    
    void Awake() {
        _inst = this;
    }

    public bool IsMute { get { return _inst._mainEffect.mute; } }
    public void Mute()
    {
        for (int i = 0; i < _soundSources.Length; ++i)
            _soundSources[i].mute = true;
        _mainEffect.mute = true;
    }
    public void UnMute()
    {
        for (int i = 0; i < _soundSources.Length; ++i)
            _soundSources[i].mute = false;
        _mainEffect.mute = false;
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlayMusic(eMusicType type)
    {
        if(_musicSource.isPlaying)
            return;
        _musicSource.clip = _musics[(int)type];
        _musicSource.loop = false;
        _musicSource.Play();
    }

    const float de = 0.043f;

    delay _delaySound0 = new delay(de);
    delay[] _delaySound = new delay[] {
        new delay(de), new delay(de), new delay(de), new delay(de),
        new delay(de), new delay(de), new delay(de), new delay(de),
        new delay(de), new delay(de), new delay(de), new delay(de),
    };
    
    int _soundIdx = 0;
    public static void Play(eSoundType type, float pitch = 1.0f)
    {
        if (!_inst._delaySound0.IsEnd())
            return;
        _inst._delaySound0.Reset();
        _inst._soundSources[_inst._soundIdx].clip = _inst._sounds[(int)type];
        _inst._soundSources[_inst._soundIdx].pitch = pitch;
       //     _soundSources[_soundIdx].pitch = pitch_;
        _inst._soundSources[_inst._soundIdx].Play();
        
        ++_inst._soundIdx;
        _inst._soundIdx = _inst._soundIdx % _inst._soundSources.Length;
    }
}
