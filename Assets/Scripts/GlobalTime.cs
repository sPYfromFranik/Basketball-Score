using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTime : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI hoursText;
    [SerializeField] TextMeshProUGUI minutesText;

    // Update is called once per frame
    void Update()
    {
        hoursText.text = System.DateTime.Now.Hour < 10 ? '0' + System.DateTime.Now.Hour.ToString() : System.DateTime.Now.Hour.ToString();
        minutesText.text = System.DateTime.Now.Minute < 10 ? '0' + System.DateTime.Now.Minute.ToString() : System.DateTime.Now.Minute.ToString();
    }
}
