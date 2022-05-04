using UnityEngine;

public class app : MonoBehaviour
{    
    void Awake()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        if(!IsVendingApk())
            Application.Quit();
#else
#endif
        UnityEngineEx.RandEx.InitSeedWithTick();
        Application.targetFrameRate = 36;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            uis.pops.BackBtn.Show();
    }

    public static bool IsVendingApk()
    {
        return Application.installerName.Equals("com.android.vending");
    }
    
}