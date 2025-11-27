using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName; // 인스펙터에서 이동할 씬 이름 입력

    private bool isPlayerInPortal = false; // 플레이어가 포탈 안에 있는지 체크

    void Update()
    {
        // 플레이어가 포탈 범위 안에 있고 + 'E' 키를 눌렀을 때
        if (isPlayerInPortal && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[Portal] {nextSceneName} 씬으로 이동합니다.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 플레이어가 포탈 영역에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            Debug.Log("포탈 진입! 이동하려면 'E' 키를 누르세요.");
        }
    }

    // 플레이어가 포탈 영역에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
        }
    }
}