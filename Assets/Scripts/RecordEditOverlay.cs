using TMPro;
using UnityEngine;

public class RecordEditOverlay : MonoBehaviour
{
    [HideInInspector] public Record editingRecord;
    [SerializeField] TMP_Dropdown leftTeamDropdown;
    [SerializeField] TMP_Dropdown rightTeamDropdown;
    [SerializeField] TMP_InputField leftTeamScore;
    [SerializeField] TMP_InputField rightTeamScore;
    [SerializeField] TMP_InputField leftTeamFouls;
    [SerializeField] TMP_InputField rightTeamFouls;

    private void OnEnable()
    {
        ScreensOrganizer.recordEditOverlayOpen = true;
    }

    private void OnDisable()
    {
        ScreensOrganizer.recordEditOverlayOpen = false;
    }

    public void UpdateVisuals()
    {
        leftTeamDropdown.value = editingRecord.scoreHistoryRecord.leftTeam.team.ID;
        rightTeamDropdown.value = editingRecord.scoreHistoryRecord.rightTeam.team.ID;
        leftTeamScore.text = editingRecord.scoreHistoryRecord.leftTeam.score.ToString();
        rightTeamScore.text = editingRecord.scoreHistoryRecord.rightTeam.score.ToString();
        leftTeamFouls.text = editingRecord.scoreHistoryRecord.leftTeam.fouls.ToString();
        rightTeamFouls.text = editingRecord.scoreHistoryRecord.rightTeam.fouls.ToString();
    }

    public void SaveChanges()
    {
        editingRecord.scoreHistoryRecord.leftTeam.team = FindObjectOfType<ScoreCounter>().teams[leftTeamDropdown.value];
        editingRecord.scoreHistoryRecord.rightTeam.team = FindObjectOfType<ScoreCounter>().teams[rightTeamDropdown.value];
        editingRecord.scoreHistoryRecord.leftTeam.score = int.Parse(leftTeamScore.text);
        editingRecord.scoreHistoryRecord.rightTeam.score = int.Parse(rightTeamScore.text);
        editingRecord.scoreHistoryRecord.leftTeam.fouls = int.Parse(leftTeamFouls.text);
        editingRecord.scoreHistoryRecord.rightTeam.fouls = int.Parse(rightTeamFouls.text);
        gameObject.SetActive(false);
        ScreensOrganizer.screensOrganizer.historyOverlay.SetActive(true);
    }
}
