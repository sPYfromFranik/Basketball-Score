using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    [SerializeField] GameObject recordPrefab;
    private List<Record> recordsList = new List<Record>();
    [SerializeField] GameObject recordsHolder;
    [SerializeField] Button clearListButton;
    private void OnEnable()
    {
        FormList();
    }


    void UpdateList(Record _record)
    {
        foreach (var record in recordsList)
        {
            bool _recordFound = false;
            if (record == _record)
            {
                foreach (var record2 in ScoreCounter.history)
                {
                    if (record2.gameNumber == record.gameOrder)
                    {
                        ScoreCounter.history.Remove(record2);
                        _recordFound = true;
                        break;
                    }
                }
            }
            if (_recordFound)
                break;
        }
        for (int i = 0; i < ScoreCounter.history.Count; i++) 
        {
            ScoreCounter.history[i].gameNumber = i;
        }
        ClearList();
        FormList();
    }

    void FormList()
    {
        foreach (var record in ScoreCounter.history)
        {
            Instantiate(recordPrefab, recordsHolder.transform);
        }
        List<Record> _records = recordsHolder.GetComponentsInChildren<Record>().ToList();
        if (_records.Count != ScoreCounter.history.Count)
            Debug.LogError("records items and history count is not equal. wtf?");
        if (_records.Count > 0)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                recordsList.Add(_records[i]);
                _records[i].Deleted += UpdateList;
                _records[i].gameOrder = ScoreCounter.history[i].gameNumber;
            }
        }
        recordsHolder.GetComponent<VerticalLayoutGroup>().spacing = 1;
        recordsHolder.GetComponent<VerticalLayoutGroup>().spacing = 0;
        if (recordsList.Count > 0) 
            clearListButton.interactable = true;
        else
            clearListButton.interactable = false;
        UpdateVisuals();
    }

    public void ClearList()
    {
        if (recordsList.Count > 0)
            for (int i = 0; i < recordsList.Count; i++)
            {
                DestroyImmediate(recordsList[i].gameObject);
                DestroyImmediate(recordsList[i]);
            }
        recordsList.Clear();
    }

    public void ClearHistory()
    {
        ClearList();
        ScoreCounter.history.Clear();
        clearListButton.interactable = false;
    }

    private void UpdateVisuals()
    {
        var _i = 0;
        foreach (Record record in recordsList)
        {
            record.gameOrder = _i;
            record.leftScore.text = ScoreCounter.history[_i].leftTeam.score.ToString();
            record.rightScore.text = ScoreCounter.history[_i].rightTeam.score.ToString();
            record.leftTeamImage.sprite = ScoreCounter.history[_i].leftTeam.team.icon;
            record.rightTeamImage.sprite = ScoreCounter.history[_i].rightTeam.team.icon;
            _i++;
        }
    }

    public void CloseHistory()
    {
        ClearList();
        this.gameObject.SetActive(false);
    }
}