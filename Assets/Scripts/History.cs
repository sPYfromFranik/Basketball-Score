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
    [SerializeField] GameObject recordEditOverlay;
    public static bool historyOpened;
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
                _records[i].Editing += UpdateRecord;
                _records[i].gameOrder = ScoreCounter.history[i].gameNumber;
            }
        }
        var _i = 0;
        foreach (Record record in recordsList)
        {
            record.gameOrder = _i;
            record.scoreHistoryRecord = ScoreCounter.history[_i];
            record.SetVisuals();
            _i++;
        }
        if (recordsList.Count > 0) 
            clearListButton.interactable = true;
        else
            clearListButton.interactable = false;
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
        Vibration.VibratePop();
        ClearList();
        ScoreCounter.history.Clear();
        clearListButton.interactable = false;
    }

    public static void CloseHistory()
    {
        historyOpened = false;
        Vibration.VibratePop();
        FindObjectOfType<History>().ClearList();
        FindObjectOfType<History>().gameObject.SetActive(false);
    }

    private void UpdateRecord(Record _record)
    {
        RecordEditOverlay.recordEditOverlayOpened = true;
        recordEditOverlay.SetActive(true);
        FindObjectOfType<RecordEditOverlay>().editingRecord = _record;
        FindObjectOfType<RecordEditOverlay>().UpdateVisuals();
        CloseHistory();
    }
}
