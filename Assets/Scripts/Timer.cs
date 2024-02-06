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
    [SerializeField] TMP_Text before;
    [SerializeField] TMP_Text after;

    private bool gameStarted=false;
    private bool gamePaused=true;
    private float timeLeft;
    private float timer;
    private System.DateTime pauseDateTime;
    // Start is called before the first frame update
    void Start()
    {
        startNewGameButton.interactable = false;
        timer = PlayerPrefs.GetInt("Timer", 600);
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
                UpdateTimer();
        }
        if (gameStarted && !gamePaused && timeLeft <= 0)
        {
            //PlayOneShot();
            Debug.Log("Time ended");
            timeLeft = 0;
            gamePaused = true;
            pauseButtonText.text = "⏵";
            UpdateTimer();
            gamePaused = true;
        }
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
            timeLeft += _time;

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
        }

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
        UpdateTimer();
    }
}
