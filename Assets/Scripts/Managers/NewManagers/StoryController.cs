using UnityEngine;

public class StoryController : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public void StartGame()
    {
        mainMenuManager.LoadMainScene();
    }
}
