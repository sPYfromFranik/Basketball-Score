using UnityEngine;

public class SaveGameOverlay : MonoBehaviour
{
    private void OnEnable()
    {
        ScreensOrganizer.endGameOverlayOpen = true;
    }

    private void OnDisable()
    {
        ScreensOrganizer.endGameOverlayOpen = false;
    }
}
