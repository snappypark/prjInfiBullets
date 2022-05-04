using UnityEngine;

public static partial class langs
{
    public static string Ok()  { switch (_lang)  { case SystemLanguage.Korean: return "확인"; case SystemLanguage.Japanese: return "Ok"; default: return "Ok"; } }
    public static string Yes()  { switch (_lang)  { case SystemLanguage.Korean: return "네"; case SystemLanguage.Japanese: return "はい"; default: return "Yes"; } }
    public static string No() {  switch (_lang) { case SystemLanguage.Korean: return "아니오"; case SystemLanguage.Japanese: return "いいえ"; default: return "No"; } }
    public static string Cancel() {  switch (_lang) { case SystemLanguage.Korean: return "취소"; case SystemLanguage.Japanese: return "キャンセル"; default: return "Cancel"; } }

    
    public static string Option_language() { switch (_lang) {
            case SystemLanguage.Korean: return "언어";
            case SystemLanguage.Japanese: return "言語";
            default: return "language"; } }

    public static string ExitGame() { switch (_lang) {
            case SystemLanguage.Korean: return "게임을 종료하시겠습니까?";
            case SystemLanguage.Japanese: return "ゲームを終了しますか？";
            default: return "QUIT GAME?"; }  }

    public static string UninstallLoss() {
        switch (_lang) {
            case SystemLanguage.Korean: return "앱을 제거시 게임 데이터가 저장되지 않습니다.";
            case SystemLanguage.Japanese: return "Game data cannot be recovered once the app is deleted.";
            default: return "Game data cannot be recovered once the app is deleted.";
        }
    }

    public static string Rate() { switch (_lang) {
            case SystemLanguage.Korean: return "평가";
            case SystemLanguage.Japanese: return "Rate";
            default: return "Rate"; } }

    public static string rateUs()
    {
        switch (_lang)
        {
            case SystemLanguage.Korean: return "평가하기";
            case SystemLanguage.Japanese: return "RATE US";
            default: return "RATE US";
        }
    }
    public static string tryNow()
    {
        switch (_lang)
        {
            case SystemLanguage.Korean: return "추천게임";
            case SystemLanguage.Japanese: return "TRY NOW";
            default: return "TRY NOW";
        }
    }

    
    public static string adIsNotReady()
    {
        switch (_lang)
        {
            case SystemLanguage.Korean: return "광고가 준비되어 있지 않습니다.";
            case SystemLanguage.Japanese: return "Ad is not ready.";
            default: return "Ad is not ready.";
        }
    }
}
