using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Map1"; // 게임 시작 시 이동할 씬 이름
    public string tutorialSceneName = "Tutorial"; // 튜토리얼 씬 이름
    public string settingsSceneName = "Settings"; // 설정 씬 이름

    public void OnClickStart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OnClickSettings()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}