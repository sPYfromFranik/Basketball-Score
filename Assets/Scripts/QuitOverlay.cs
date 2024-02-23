using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOverlay : MonoBehaviour
{

    public static bool quitOverlayOpened;

    public static void CloseQuitOverlay()
    {
        quitOverlayOpened = false;
        Vibration.VibratePop();
        FindObjectOfType<QuitOverlay>().gameObject.SetActive(false);
    }
}
