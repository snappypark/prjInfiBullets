using UnityEngine;

public struct sc_static
{
    static bool s_popupOpened = false;

    internal static void ProblemDetected(string message)
    {
        // 보안 문제
        Time.timeScale = 0.0f;
        OpenSecurePopup();
    }

    static void OpenSecurePopup()
    {
        if (s_popupOpened == false)
        {
            s_popupOpened = true;
            // 강제 종료
        }
    }
}
