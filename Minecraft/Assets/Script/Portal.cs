using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string nextSceneName = "Map2"; // 이동할 씬 이름 (인스펙터에서 설정)
    public float delayTime = 2.0f;        // 포탈 대기 시간 (초)

    private float timer = 0f;
    private bool isPlayerIn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            timer = 0f; // 타이머 초기화
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerIn && other.CompareTag("Player"))
        {
            timer += Time.deltaTime;

            // 대기 시간이 지나면 씬 이동
            if (timer >= delayTime)
            {
                Debug.Log($"[Portal] {nextSceneName} 이동 중...");
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
            timer = 0f;
        }
    }
}