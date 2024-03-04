using System.Collections;
using UnityEditor;
using UnityEngine;

public class ScreensOrganizer : MonoBehaviour
{
    public static bool menuOverlayOpen;
    public static bool quitOverlayOpen;
    public static bool historyOverlayOpen;
    public static bool statisticsOverlayOpen;
    public static bool recordEditOverlayOpen;
    public static bool endGameOverlayOpen;

    public static ScreensOrganizer screensOrganizer;

    [SerializeField] GameObject menuOverlay;
    [SerializeField] GameObject quitOverlay;
    public GameObject historyOverlay;
    [SerializeField] GameObject statisticsOverlay;
    public GameObject recordEditOverlay;
    [SerializeField] GameObject endGameOverlay;

    private bool quitBool = false;

    private void Awake()
    {
        screensOrganizer = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuOverlayOpen == true)
                menuOverlay.gameObject.SetActive(false);
            else if (historyOverlayOpen == true)
                historyOverlay.gameObject.SetActive(false);
            else if (quitOverlayOpen == true)
                quitOverlay.gameObject.SetActive(false);
            else if (endGameOverlayOpen == true)
                endGameOverlay.gameObject.SetActive(false);
            else if (recordEditOverlayOpen == true)
            {
                recordEditOverlay.gameObject.SetActive(false);
                historyOverlay.gameObject.SetActive(true);
            }
            else if (statisticsOverlayOpen == true)
                statisticsOverlay.gameObject.SetActive(false);
            else if (!quitBool)
            {
                quitBool = true;
#if UNITY_EDITOR
                Debug.Log("Тапніть ще раз щоб закрити застосунок");
#else
                _ShowAndroidToastMessage("Тапніть ще раз щоб закрити застосунок");
#endif
                StartCoroutine(CloseButtonDelay());
            }
            else
            {
                quitBool = false;
                CloseApp();
            }
            Vibration.VibratePop();
        }
        else if (Input.touchCount >= 1 && quitBool)
            quitBool = false;
    }

    private IEnumerator CloseButtonDelay()
    {
        yield return new WaitForSeconds(2.5f);
        if (quitBool)
            quitBool = false;
    }

    public void VibrationPop()
    {
        Vibration.VibratePop();
    }

    public void CloseApp()
    {
        Vibration.VibratePop();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Menu.MoveAndroidApplicationToBack();
#endif
        quitOverlay.gameObject.SetActive(false);
    }

    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
