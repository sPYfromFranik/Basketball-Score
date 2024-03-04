using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text minutesText;
    [SerializeField] TMP_Text secondsText;
    [SerializeField] TMP_Text milisecondsText;
    [SerializeField] TMP_Text pauseButtonText;
    [SerializeField] Button startNewGameButton;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip secondsClip;
    [SerializeField] AudioClip endGameClip;
    [SerializeField] AudioClip midGameClip;

    public static int beepSeconds;
    private int prevBeepTime = 0;
    private bool gameStarted=false;
    private bool gamePaused=true;
    private float timeLeft;
    private float timer;
    private System.DateTime pauseDateTime;
    private bool midGameSoundPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        Vibration.Init();
        startNewGameButton.interactable = false;
        timer = PlayerPrefs.GetInt("Timer", 600);
        beepSeconds = PlayerPrefs.GetInt("BeepSeconds", 10);
        timeLeft = timer;
        UpdateTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !gamePaused)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0)
            {
                UpdateTimer();
                if (midGameSoundPlayed && timeLeft > timer / 2)
                    midGameSoundPlayed = false;
                if (timeLeft <= timer / 2 && PlayerPrefs.GetInt("MidGameSound", 1) == 1 && !midGameSoundPlayed)
                {
                    audioSource.PlayOneShot(midGameClip);
                    if (PlayerPrefs.GetInt("SoundVibration", 1) == 1)
                        Vibration.Vibrate();
                    midGameSoundPlayed = true;
                }
                PlaySecondsSound();
            }   
        }
        if (gameStarted && !gamePaused && timeLeft <= 0)
        {
            PlayEndSound();
            Debug.Log("Time ended");
            timeLeft = 0;
            gamePaused = true;
            pauseButtonText.text = "⏵";
            UpdateTimer();
            gamePaused = true;
        }
    }
    public void PlayEndSound()
    {
        audioSource.PlayOneShot(endGameClip);
        if (PlayerPrefs.GetInt("SoundVibration", 1) == 1)
            Vibration.Vibrate();
    }

    public void AddTime(int _time)
    {
        if (!gameStarted)
        {
            timer += _time;
            PlayerPrefs.SetInt("Timer", (int)timer);
            timeLeft = timer;
        }
        else if (gameStarted)
        {
            timeLeft += _time;
            prevBeepTime = (int)Mathf.Floor(timeLeft % 60);
        }
        Vibration.VibratePop();
        UpdateTimer();
    }

    public void RemoveTime(int _time)
    {
        if (!gameStarted)
        {
            timer -= _time;
            timer = timer < 0 ? 0 : timer;
            PlayerPrefs.SetInt("Timer", (int)timer);
            timeLeft = timer;
        }
        else if (gameStarted)
        {
            timeLeft -= _time;
            timeLeft = timeLeft < 0 ? 0 : timeLeft;
            prevBeepTime = (int)Mathf.Floor(timeLeft % 60);
        }
        Vibration.VibratePop();
        UpdateTimer();
    }

    public void PauseButton()
    {
        if (gamePaused && timeLeft > 0)
        {
            gamePaused = false;
            pauseButtonText.text = "⏸";
            if (!gameStarted)
            {
                startNewGameButton.interactable = true;
                gameStarted = true;
            }
        }
        else if (!gamePaused && timeLeft > 0)
        {
            gamePaused = true;
            pauseButtonText.text = "⏵";
        }
        Vibration.VibratePop();
    }

    private void UpdateTimer()
    {
        float minutes = timeLeft / 60;
        minutesText.text = minutes < 10 ? "0" + Mathf.Floor(minutes).ToString() : Mathf.Floor(minutes).ToString();
        float seconds = timeLeft % 60;
        secondsText.text = seconds < 10 ? "0" + Mathf.Floor(seconds).ToString() : Mathf.Floor(seconds).ToString();
        float miliseconds = seconds % 1 * 100;
        milisecondsText.text = miliseconds < 10 ? "0" + Mathf.Floor(miliseconds).ToString() : Mathf.Floor(miliseconds).ToString();

    }

    private void PlaySecondsSound()
    {
        if (timeLeft < beepSeconds && timeLeft > 0)
            if (prevBeepTime != (int)Mathf.Floor(timeLeft % 60))
            {
                prevBeepTime = (int)Mathf.Floor(timeLeft % 60);
                audioSource.PlayOneShot(secondsClip);
                if (PlayerPrefs.GetInt("SoundVibration", 1) == 1)
                    Vibration.VibrateNope();
            }
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause && gameStarted && !gamePaused)
        {
            pauseDateTime = System.DateTime.Now;
        }
        if (!pause && gameStarted && !gamePaused)
        {
            System.TimeSpan timeSpent = pauseDateTime - System.DateTime.Now;
            double timePassed = timeSpent.TotalSeconds;
            if (timePassed < 0)
            {
                timeLeft += (float)timePassed;
                UpdateTimer();
            }
        }
    }

    public void Reset()
    {
        startNewGameButton.interactable = false;
        gameStarted = false;
        gamePaused = true;
        timer = PlayerPrefs.GetInt("Timer", 600);
        pauseButtonText.text = "⏵";
        timeLeft = timer;
        prevBeepTime = (int)Mathf.Floor(timeLeft);
        UpdateTimer();
    }
}
