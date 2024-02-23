using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameOverlay : MonoBehaviour
{
    public static bool endGameOverlayOpened;

    public static void CloseEndGameOverlay()
    {
        endGameOverlayOpened = false;
        FindObjectOfType<EndGameOverlay>().gameObject.SetActive(false);
    }
}
