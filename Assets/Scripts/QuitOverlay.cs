using UnityEngine;

public class QuitOverlay : MonoBehaviour
{
    private void OnEnable()
    {
        ScreensOrganizer.quitOverlayOpen = true;
    }

    private void OnDisable()
    {
        ScreensOrganizer.quitOverlayOpen = false;
    }
}
