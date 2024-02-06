using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject historyOverlay;

    public void OpenHistory()
    {
        historyOverlay.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void Donate()
    {
        Application.OpenURL("https://send.monobank.ua/jar/3Q4pR2QGyK");
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
