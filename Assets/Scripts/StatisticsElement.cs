using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsElement : MonoBehaviour
{
    [SerializeField] List<Sprite> placeImages;

    [SerializeField] Image bibImage;
    [SerializeField] TMP_Text teamName;
    [SerializeField] Image placeImage;

    [SerializeField] TMP_Text gamesPlayed;
    [SerializeField] TMP_Text gamesWon;
    [SerializeField] TMP_Text gamesLost;
    [SerializeField] TMP_Text gamesPercentage;

    [SerializeField] TMP_Text scoreTeamTotal;
    [SerializeField] TMP_Text scoreTeamAvg;
    [SerializeField] TMP_Text scoreEnemyTotal;
    [SerializeField] TMP_Text scoreEnemyAvg;

    [SerializeField] TMP_Text foulsTotal;
    [SerializeField] TMP_Text foulsAverage;
    [SerializeField] TMP_Text foulsPercentage;

    public void UpdateVisuals (StatisticsOverlay.StatisticsRecord record)
    {
        bibImage.sprite = record.team.icon;
        teamName.text = record.team.name.ToString();
        if (record.place >= 4)
            placeImage.gameObject.SetActive(false);
        else
            placeImage.sprite = placeImages[record.place - 1];
        
        gamesPlayed.text = record.gamesPlayed.ToString();
        gamesWon.text = record.gamesWon.ToString();
        gamesLost.text = record.gamesLost.ToString();
        gamesPercentage.text = record.gamesPercentage.ToString("F2");

        scoreTeamTotal.text = record.scoreTeamTotal.ToString();
        scoreTeamAvg.text = record.scoreTeamAvg.ToString("F2");
        scoreEnemyTotal.text = record.scoreEnemyTotal.ToString();
        scoreEnemyAvg.text = record.scoreEnemyAvg.ToString("F2");

        foulsTotal.text = record.foulsTotal.ToString();
        foulsAverage.text = record.foulsAverage.ToString("F2");
        foulsPercentage.text = record.foulsPercentage.ToString("F2");
    }
}
