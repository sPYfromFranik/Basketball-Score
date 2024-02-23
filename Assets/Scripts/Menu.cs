using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject historyOverlay;
    [SerializeField] GameObject quitOverlay;
    [SerializeField] TMP_InputField beepSecondsInput;
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Toggle midGameSoundBool;
    [SerializeField] Toggle vibroFeedback;
    [SerializeField] Toggle soundVibration;
    public static bool menuOpened;
    private bool firstLoadHappened = false;

    void Awake()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1);
        beepSecondsInput.text = PlayerPrefs.GetInt("BeepSeconds", 10).ToString();
        midGameSoundBool.isOn = PlayerPrefs.GetInt("MidGameSound", 1) == 1 ? true : false;
        vibroFeedback.isOn = PlayerPrefs.GetInt("VibroFeedback", 1) == 1 ? true : false;
        soundVibration.isOn = PlayerPrefs.GetInt("SoundVibration", 1) == 1 ? true : false;
        firstLoadHappened = true;
    }

    public void OpenHistory()
    {
        historyOverlay.SetActive(true);
        History.historyOpened = true;
        Vibration.VibratePop();
        CloseMenu();
    }

    public static void CloseMenu()
    {
        menuOpened = false;
        Vibration.VibratePop();
        FindObjectOfType<Menu>().gameObject.SetActive(false);
    }

    public void Donate()
    {
        Vibration.VibratePop();
        Application.OpenURL("https://send.monobank.ua/jar/3Q4pR2QGyK");
    }

    public static void CloseGame()
    {
        Vibration.VibratePop();
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
        Vibration.VibratePop();
        QuitOverlay.quitOverlayOpened = true;
        quitOverlay.SetActive(true);
        CloseMenu();
    }

    public static void MoveAndroidApplicationToBack()
    {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }

    public void UpdateBeepSeconds()
    {
        int _inputResult = int.Parse(beepSecondsInput.text) < 0 ? 0 : int.Parse(beepSecondsInput.text);
        PlayerPrefs.SetInt("BeepSeconds", _inputResult);
        Timer.beepSeconds = _inputResult;
        if (firstLoadHappened)
            Vibration.VibratePop();
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        audioMixer.SetFloat("MasterVolume", volumeSlider.value);
        if (firstLoadHappened)
        {
            FindObjectOfType<Timer>().PlayEndSound();
            Vibration.VibratePop();
        }
    }

    public void UpdateMidGameSoundBool()
    {
        PlayerPrefs.SetInt("MidGameSound", midGameSoundBool.isOn ? 1 : 0);
        if (firstLoadHappened)
            Vibration.VibratePop();
    }

    public void UpdateVibroFeedback()
    {
        PlayerPrefs.SetInt("VibroFeedback", vibroFeedback.isOn ? 1 : 0);
        if (firstLoadHappened)
            Vibration.VibratePop();
    }

    public void UpdateSoundVibration()
    {
        PlayerPrefs.SetInt("SoundVibration", soundVibration.isOn ? 1 : 0);
        if (firstLoadHappened)
            Vibration.VibratePop();
    }
}
