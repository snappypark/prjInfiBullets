using UnityEngine;

public static partial class langs
{
    static SystemLanguage _lang { get {
#if UNITY_EDITOR
            return SystemLanguage.English; //Japanese; English;
#else
            return Application.systemLanguage;
#endif
        }
    }

    public static string Version()
    {
        return string.Format("v{0}", Application.version);
    }
    
    public static string MoveJoystic()
    {
        switch (_lang) {
            case SystemLanguage.Korean: return "조이스틱을 움직여주세요.";
            case SystemLanguage.Japanese: return "ジョイスティックを使って遊ぶ.";
            default: return "MOVE THE JOYSTICK TO PLAY";
        }
    }

    public static string Stage(int value, int max)
    {
        if (value >= max)
            return string.Format("THE END #{0}", value) ;
        switch (_lang) {
            case SystemLanguage.Korean: return string.Format("스테이지 #{0}", value);
            case SystemLanguage.Japanese: return string.Format("ステージ #{0}", value);
            default: return string.Format("STAGE #{0}", value);
        }
    }
    
    public static string StageSharp(int value, int max)
    {
        if (value >= max)
            return string.Format("THE END #{0}", value) ;
        return string.Format("#{0}", value);
    }
    
    public static string GameClear(){
        switch (_lang){
            case SystemLanguage.Korean: return "게임 클리어";
            case SystemLanguage.Japanese: return "ゲーム クリア";
            default: return "GAME CLEAR";
        }
    }

    public static string GameOver() {
        switch (_lang){
            case SystemLanguage.Korean: return "게임 오버";
            case SystemLanguage.Japanese: return "ゲーム オーバー";
            default: return "GAME OVER";
        }
    }

    public static string LvUp(int curLv, int LastLv)
    {
        return curLv >= LastLv ? "Max Level" : "Level Up";
    }


    
}
