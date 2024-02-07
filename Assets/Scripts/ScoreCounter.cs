using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TMP_Text leftScoreText;
    [SerializeField] TMP_Text rightScoreText;
    [SerializeField] Button leftMinus1;
    [SerializeField] Button leftPlus1;
    [SerializeField] Button leftPlus2;
    [SerializeField] Button leftPlus3;
    [SerializeField] Button rightMinus1;
    [SerializeField] Button rightPlus1;
    [SerializeField] Button rightPlus2;
    [SerializeField] Button rightPlus3;
    [SerializeField] Sprite noBib;
    [SerializeField] Sprite orangeBib;
    [SerializeField] Sprite greenBib;
    [SerializeField] TMP_Dropdown leftTeamDropdown;
    [SerializeField] TMP_Dropdown rightTeamDropdown;
    [SerializeField] GameObject menuOverlay;
    private bool quitBool = false;

    public List<TeamPreset> teams = new List<TeamPreset>();
    public class TeamPreset
    {
        public int ID;
        public Sprite icon;
    }
    public List<TeamScore> playingTeams = new List<TeamScore>();
    public class TeamScore : ICloneable
    {
        public TeamPreset team;
        public enum positions { left, right }
        public positions position;
        public int score = 0;

        public TeamScore(TeamPreset _team, positions _position, int _score)
        {
            team = _team;
            position = _position;
            score = _score;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static List<ScoreHistoryRecord> history = new List<ScoreHistoryRecord>();
    public class ScoreHistoryRecord
    {
        public int gameNumber;
        public TeamScore leftTeam;
        public TeamScore rightTeam;
    }

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        for (int i = 0; i < 3; i++)
            teams.Add(new TeamPreset());
        for (int i = 0; i < teams.Count; i++)
            teams[i].ID = i;
        teams[0].icon = noBib;
        teams[1].icon = orangeBib;
        teams[2].icon = greenBib;
    }

    private void Start()
    {
        playingTeams.Add(new TeamScore(teams[leftTeamDropdown.value], TeamScore.positions.left, 0));
        playingTeams.Add(new TeamScore(teams[rightTeamDropdown.value], TeamScore.positions.right, 0));
    }

    void OnEnable()
    {
        leftMinus1.onClick.AddListener(() => UpdateScore(-1, TeamScore.positions.left));
        leftPlus1.onClick.AddListener(() => UpdateScore(1, TeamScore.positions.left));
        leftPlus2.onClick.AddListener(() => UpdateScore(2, TeamScore.positions.left));
        leftPlus3.onClick.AddListener(() => UpdateScore(3, TeamScore.positions.left));
        rightMinus1.onClick.AddListener(() => UpdateScore(-1, TeamScore.positions.right));
        rightPlus1.onClick.AddListener(() => UpdateScore(1, TeamScore.positions.right));
        rightPlus2.onClick.AddListener(() => UpdateScore(2, TeamScore.positions.right));
        rightPlus3.onClick.AddListener(() => UpdateScore(3, TeamScore.positions.right));
        leftTeamDropdown.onValueChanged.AddListener((int _temp) => ChangePlayingTeam(TeamScore.positions.left, leftTeamDropdown.value));
        rightTeamDropdown.onValueChanged.AddListener((int _temp) => ChangePlayingTeam(TeamScore.positions.right, rightTeamDropdown.value));
    }

    public void UpdateScore(int _score, TeamScore.positions _teamPosition)
    {
        foreach (var team in playingTeams)
            if (team.position == _teamPosition)
            {
                team.score += _score;
                team.score = team.score < 0 ? 0 : team.score;
                break;
            }
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        foreach (var team in playingTeams)
            if (team.position == TeamScore.positions.left)
                leftScoreText.text = team.score < 10 ? "0" + team.score.ToString() : team.score.ToString();
            else
                rightScoreText.text = team.score < 10 ? "0" + team.score.ToString() : team.score.ToString();
    }
    public void Reset()
    {
        AddToHistory();
        foreach (var team in playingTeams)
            team.score = 0;
        UpdateScoreText();
    }

    public void ChangePlayingTeam(TeamScore.positions _side, int _team)
    {
        foreach (var team in playingTeams)
            if (team.position == _side)
                foreach (var teamGlobal in teams)
                    if (teamGlobal.ID == _team)
                        team.team = teamGlobal;
    }

    public void AddToHistory()
    {
        history.Add(new ScoreHistoryRecord());
        int _posInHistory = history.Count - 1;
        history[_posInHistory].gameNumber = _posInHistory;
        foreach (var team in playingTeams)
            if (team.position == TeamScore.positions.left)
                history[_posInHistory].leftTeam = (TeamScore)team.Clone();
            else if (team.position == TeamScore.positions.right)
                history[_posInHistory].rightTeam = (TeamScore)team.Clone();
    }

    public void OpenMenu()
    {
        menuOverlay.SetActive(true);
        Menu.menuOpened = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu.menuOpened == true)
                Menu.CloseMenu();
            else if (History.historyOpened == true)
                History.CloseHistory();
            else if (QuitOverlay.quitOverlayOpened == true)
                QuitOverlay.CloseQuitOverlay();
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
                Menu.CloseGame();
            }
                
        }
        else if (Input.touchCount >= 1 && quitBool)
            quitBool = false;
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

    private IEnumerator CloseButtonDelay()
    {
        yield return new WaitForSeconds(2.5f);
        if (quitBool)
            quitBool=false;
    }
}
