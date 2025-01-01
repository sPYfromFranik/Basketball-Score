using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatisticsOverlay : MonoBehaviour
{
    [SerializeField] GameObject statisticsRecordUIPrefab;
    [SerializeField] GameObject listUI;
    public class StatisticsRecord
    {
        public ScoreCounter.TeamPreset team;
        public int place = 0;
        
        public int gamesPlayed;
        public int gamesWon;
        public int gamesLost;
        public float gamesPercentage;

        public int scoreTeamTotal;
        public float scoreTeamAvg;
        public int scoreEnemyTotal;
        public float scoreEnemyAvg;

        public int foulsTotal;
        public float foulsAverage;
        public float foulsPercentage;

        public StatisticsRecord (ScoreCounter.TeamPreset _team)
        {
            team = _team;
        }
    }
    private List<StatisticsRecord> statisticsRecords = new List<StatisticsRecord>();
    List<StatisticsElement> listOfUIElements = new List<StatisticsElement>();

    private void OnEnable()
    {
        ScreensOrganizer.statisticsOverlayOpen = true;
        FormList();
    }

    private void OnDisable()
    {
        ScreensOrganizer.statisticsOverlayOpen = false;
        ClearList();
    }


    public void ClearList()
    {
        if (listOfUIElements.Count > 0)
            for (int i = 0; i < listOfUIElements.Count; i++)
            {
                DestroyImmediate(listOfUIElements[i].gameObject);
                DestroyImmediate(listOfUIElements[i]);
            }
        listOfUIElements.Clear();
        statisticsRecords.Clear();
    }

    public void FormList()
    {
        //form a list of teams that played
        List<ScoreCounter.TeamPreset> playingTeams = new List<ScoreCounter.TeamPreset>();
        foreach (ScoreCounter.ScoreHistoryRecord scoreHistoryRecord in ScoreCounter.historyWrapper.history)
        {
            var leftTeamFound = false;
            var rightTeamFound = false;
            foreach (var playingTeam in playingTeams)
            {
                if (scoreHistoryRecord.leftTeam.team == playingTeam)
                    leftTeamFound = true;
                if (scoreHistoryRecord.rightTeam.team == playingTeam)
                    rightTeamFound = true;
            }
            if (playingTeams.Count == 0 && scoreHistoryRecord.rightTeam.team == scoreHistoryRecord.leftTeam.team)
                rightTeamFound = true;
            if (!leftTeamFound)
                playingTeams.Add(scoreHistoryRecord.leftTeam.team);
            if (!rightTeamFound)
                playingTeams.Add(scoreHistoryRecord.rightTeam.team);

        }
        //create a statistics entry for each team
        foreach (var playingTeam in playingTeams)
            statisticsRecords.Add(new StatisticsRecord(playingTeam));

        //fill statistics with data
        foreach (ScoreCounter.ScoreHistoryRecord scoreHistoryRecord in ScoreCounter.historyWrapper.history)
            foreach (var statisticsTeam in statisticsRecords)
            {
                if (statisticsTeam.team == scoreHistoryRecord.leftTeam.team)
                    getData(statisticsTeam, scoreHistoryRecord.leftTeam, scoreHistoryRecord.rightTeam);
                if (statisticsTeam.team == scoreHistoryRecord.rightTeam.team)
                    getData(statisticsTeam, scoreHistoryRecord.rightTeam, scoreHistoryRecord.leftTeam);
            }

        //calculate total number of fouls
        int totalFouls = 0;
        foreach (var statisticsTeam in statisticsRecords)
            totalFouls += statisticsTeam.foulsTotal;

        //calculate averages and percentages
        foreach (var statisticsTeam in statisticsRecords)
        {
            statisticsTeam.gamesPercentage = (float)statisticsTeam.gamesWon / statisticsTeam.gamesPlayed * 100;
            statisticsTeam.scoreTeamAvg = (float) statisticsTeam.scoreTeamTotal / statisticsTeam.gamesPlayed;
            statisticsTeam.scoreEnemyAvg = (float) statisticsTeam.scoreEnemyTotal / statisticsTeam.gamesPlayed;
            statisticsTeam.foulsAverage = (float) statisticsTeam.foulsTotal / statisticsTeam.gamesPlayed;
            statisticsTeam.foulsPercentage = totalFouls!=0 ? (float) statisticsTeam.foulsTotal / totalFouls * 100 : 0;
        }

        //define positions
        //form a list of all % won independent of the team
        List<float> places = new List<float>();
        foreach (var statisticsTeam in statisticsRecords)
        {
            var placefound = false;
            foreach (float place in places)
            {
                if (statisticsTeam.gamesPercentage == place)
                    placefound = true;
            }
            if (!placefound)
                places.Add(statisticsTeam.gamesPercentage);
        }
        //sort the list from big to small
        places.Sort();
        places.Reverse();
        //assign teams their places depending on the position of % won games among other records
        foreach (var statisticsTeam in statisticsRecords)
            for (int i = 0; i < places.Count; i++)
                if (statisticsTeam.gamesPercentage == places[i])
                    statisticsTeam.place = i + 1;
        //create gameobjects and fill them with data
        foreach (var statisticsTeam in statisticsRecords)
            Instantiate(statisticsRecordUIPrefab, listUI.transform);
        listOfUIElements = listUI.GetComponentsInChildren<StatisticsElement>().ToList();
        for (int i = 0; i < listOfUIElements.Count; i++)
        {
            int smallestPlace = statisticsRecords[0].place;
            int teamIDInStatistics = 0;
            for (int j = 0; j < statisticsRecords.Count; j++)
            {
                if (smallestPlace > statisticsRecords[j].place)
                {
                    smallestPlace = statisticsRecords[j].place;
                    teamIDInStatistics = j;
                }
            }
            listOfUIElements[i].UpdateVisuals(statisticsRecords[teamIDInStatistics]);
            statisticsRecords.RemoveAt(teamIDInStatistics);
        }
            
    }
    private void getData(StatisticsRecord _statisticsTeam, ScoreCounter.TeamScore _historyTeam, ScoreCounter.TeamScore _historyEnemy)
    {
        _statisticsTeam.gamesPlayed++;
        if (_historyTeam.score > _historyEnemy.score)
            _statisticsTeam.gamesWon++;
        else if (_historyTeam.score < _historyEnemy.score)
            _statisticsTeam.gamesLost++;
        _statisticsTeam.scoreTeamTotal += _historyTeam.score;
        _statisticsTeam.scoreEnemyTotal += _historyEnemy.score;
        _statisticsTeam.foulsTotal += _historyTeam.fouls;
    }
}
