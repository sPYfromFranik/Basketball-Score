using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject historyOverlay;
    [SerializeField] GameObject quitOverlay;
    public static bool menuOpened;

    public void OpenHistory()
    {
        historyOverlay.SetActive(true);
        History.historyOpened = true;
        CloseMenu();
    }

    public static void CloseMenu()
    {
        menuOpened = false;
        FindObjectOfType<Menu>().gameObject.SetActive(false);
    }

    public void Donate()
    {
        Application.OpenURL("https://send.monobank.ua/jar/3Q4pR2QGyK");
    }

    public static void CloseGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Menu.MoveAndroidApplicationToBack();
#endif
        if (QuitOverlay.quitOverlayOpened)
        {
            QuitOverlay.quitOverlayOpened = false;
            QuitOverlay.CloseQuitOverlay();
        }
    }

    public void OpenQuitOverlay()
    {
        QuitOverlay.quitOverlayOpened = true;
        quitOverlay.SetActive(true);
        CloseMenu();
    }

    public static void MoveAndroidApplicationToBack()
    {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }
}
