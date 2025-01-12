using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] TMP_Text leftScoreText;
    [SerializeField] TMP_Text rightScoreText;
    [SerializeField] TMP_Text leftFoulsText;
    [SerializeField] TMP_Text rightFoulsText;
    [SerializeField] Button leftFoulMinus;
    [SerializeField] Button rightFoulMinus;
    [SerializeField] Button leftFoulPlus;
    [SerializeField] Button rightFoulPlus;
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

    [SerializeField] AudioMixer audioMixer;
    public static ScoreHistoryWrapper historyWrapper;
    public static string saveFilePath;

    public List<TeamPreset> teams = new List<TeamPreset>();
    [Serializable]
    public class TeamPreset
    {
        public int ID;
        public Sprite icon;
        public string name;
        public TeamPreset(int _ID, Sprite _icon, string _name)
        {
            ID = _ID;
            icon = _icon;
            name = _name;
        }
    }
    public List<TeamScore> playingTeams = new List<TeamScore>();
    [Serializable]
    public class TeamScore : ICloneable
    {
        public TeamPreset team;
        public enum positions { left, right }
        public positions position;
        public int score = 0;
        public int fouls = 0;

        public TeamScore(TeamPreset _team, positions _position, int _score, int _fouls)
        {
            team = _team;
            position = _position;
            score = _score;
            fouls = _fouls;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    [Serializable]
    public class ScoreHistoryWrapper
    {
        public List<ScoreHistoryRecord> history = new List<ScoreHistoryRecord>();
    }

    [Serializable]
    public class ScoreHistoryRecord
    {
        public int gameNumber;
        public TeamScore leftTeam;
        public TeamScore rightTeam;
    }

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        teams.Add(new TeamPreset(0, noBib, "Без манішок"));
        teams.Add(new TeamPreset(1, orangeBib, "Оранжеві манішки"));
        teams.Add(new TeamPreset(2, greenBib, "Зелені манішки"));
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("Volume", 2));
    }

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savefile.json";
        historyWrapper = new ScoreHistoryWrapper();
        playingTeams.Add(new TeamScore(teams[leftTeamDropdown.value], TeamScore.positions.left, 0, 0));
        playingTeams.Add(new TeamScore(teams[rightTeamDropdown.value], TeamScore.positions.right, 0, 0));
        if (File.Exists(saveFilePath))
        {
            historyWrapper = JsonUtility.FromJson<ScoreHistoryWrapper>(File.ReadAllText(saveFilePath));
            //reassigning teams from the save file to the locally initiated teams
            foreach(ScoreHistoryRecord record in historyWrapper.history)
            {
                foreach (TeamPreset team in teams)
                {
                    if(record.leftTeam.team.name == team.name)
                        record.leftTeam.team = team;
                    if(record.rightTeam.team.name == team.name)
                        record.rightTeam.team = team;
                }
            }
        }
    }

    void OnEnable()
    {
        leftFoulMinus.onClick.AddListener(() => UpdateFouls(-1, TeamScore.positions.left));
        leftFoulPlus.onClick.AddListener(() => UpdateFouls(1, TeamScore.positions.left));
        rightFoulMinus.onClick.AddListener(() => UpdateFouls(-1, TeamScore.positions.right));
        rightFoulPlus.onClick.AddListener(() => UpdateFouls(1, TeamScore.positions.right));
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

    public void UpdateFouls(int _value, TeamScore.positions _teamPosition)
    {
        foreach (var team in playingTeams)
            if (team.position == _teamPosition)
            {
                team.fouls += _value;
                team.fouls = team.fouls < 0 ? 0 : team.fouls;
                break;
            }
        UpdateFoulsText();
    }

    public void UpdateFoulsText()
    {
        foreach (var team in playingTeams)
            if (team.position == TeamScore.positions.left)
                leftFoulsText.text = team.fouls.ToString();
            else
                rightFoulsText.text = team.fouls.ToString();
    }

    public void Reset()
    {
        AddToHistory();
        foreach (var team in playingTeams)
        {
            team.score = 0;
            team.fouls = 0;
        }   
        UpdateScoreText();
        UpdateFoulsText();
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
        historyWrapper.history.Add(new ScoreHistoryRecord());
        int _posInHistory = historyWrapper.history.Count - 1;
        historyWrapper.history[_posInHistory].gameNumber = _posInHistory;
        foreach (var team in playingTeams)
            if (team.position == TeamScore.positions.left)
                historyWrapper.history[_posInHistory].leftTeam = (TeamScore)team.Clone();
            else if (team.position == TeamScore.positions.right)
                historyWrapper.history[_posInHistory].rightTeam = (TeamScore)team.Clone();
        UpdateSaveFile();
#if !UNITY_EDITOR
        _ShowAndroidToastMessage("Результати гри збережені");
#endif
    }

    public static void UpdateSaveFile()
    {
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(historyWrapper));
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
