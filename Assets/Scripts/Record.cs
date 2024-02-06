using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour
{
    public event Action<Record> Deleted;
    public TMP_Text leftScore;
    public TMP_Text rightScore;
    public Image leftTeamImage;
    public Image rightTeamImage;
    [HideInInspector] public int gameOrder;

    public void DeleteRecord()
    {
        Deleted?.Invoke(this);
    }
}
