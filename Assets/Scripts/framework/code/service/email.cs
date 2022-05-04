using UnityEngine;

public class email
{
    public static void SentEvent()
    {
        string mailto = "help@90-j.com";
        string subject = EscapeURL("[line shot] Bug report / Q&A / etc");
        string body = Application.systemLanguage == SystemLanguage.Korean ? EscapeURL
            (
             "이 곳에 내용 작성 부탁합니다. :-) \n\n\n\n" +
             "________" +
             "Device Model : " + SystemInfo.deviceModel + "\n\n" +
             "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
             "________"
            ) :
            EscapeURL
            (
             "Type here what would you like. :-) \n\n\n\n" +
             "________" +
             "Device Model : " + SystemInfo.deviceModel + "\n\n" +
             "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
             "________"
            );

        Application.OpenURL("mailto:" + mailto + "?subject=" + subject + "&body=" + body);
    }

    static string EscapeURL(string url)
    {
        return null;
       // return WWW.EscapeURL(url).Replace("+", "%20");
    }
}
