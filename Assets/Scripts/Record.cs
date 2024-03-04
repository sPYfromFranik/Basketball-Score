using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour
{
    public event Action<Record> Deleted;
    public event Action<Record> Editing;
    public ScoreCounter.ScoreHistoryRecord scoreHistoryRecord;
    [SerializeField] TMP_Text leftFouls;
    [SerializeField] TMP_Text rightFouls;
    [SerializeField] TMP_Text leftScore;
    [SerializeField] TMP_Text rightScore;
    [SerializeField] Image leftTeamImage;
    [SerializeField] Image rightTeamImage;
    [HideInInspector] public int gameOrder;

    public void SetVisuals()
    {
        leftFouls.text = scoreHistoryRecord.leftTeam.fouls.ToString();
        rightFouls.text = scoreHistoryRecord.rightTeam.fouls.ToString();
        leftScore.text = scoreHistoryRecord.leftTeam.score < 10 ? '0' + scoreHistoryRecord.leftTeam.score.ToString() : scoreHistoryRecord.leftTeam.score.ToString();
        rightScore.text = scoreHistoryRecord.rightTeam.score < 10 ? '0' + scoreHistoryRecord.rightTeam.score.ToString() : scoreHistoryRecord.rightTeam.score.ToString();
        leftTeamImage.sprite = scoreHistoryRecord.leftTeam.team.icon;
        rightTeamImage.sprite = scoreHistoryRecord.rightTeam.team.icon;
    }

    public void DeleteRecord()
    {
        Vibration.VibratePop();
        Deleted?.Invoke(this);
    }

    public void EditRecord()
    {
        Vibration.VibratePop();
        Editing?.Invoke(this);
    }
}
