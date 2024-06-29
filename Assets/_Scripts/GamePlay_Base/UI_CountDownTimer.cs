using System.Collections;
using UnityEngine;
using TMPro;

public class UI_CountDownTimer : MonoBehaviour
{
    [Header("Countdown Settings")]
    [Tooltip("Countdown time in seconds")]
    public float countdownTime = 60f; // 倒计时时间，以秒为单位

    [Header("UI Elements")]
    [Tooltip("TextMeshPro Text component to display the countdown")]
    public TMP_Text countdownText; // 用于显示倒计时的 TextMeshPro 组件

    private float currentTime;

    void Start()
    {
        currentTime = countdownTime; // 初始化当前时间
        StartCoroutine(Countdown()); // 开始倒计时协程
    }

    IEnumerator Countdown()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // 每帧减少当前时间
            UpdateCountdownText(); // 更新显示的文本
            yield return null; // 等待下一帧
        }
        currentTime = 0;
        UpdateCountdownText(); // 确保最后显示 0
        OnCountdownEnd(); // 倒计时结束后的操作
    }

    void UpdateCountdownText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // 计算剩余的分钟数
        int seconds = Mathf.FloorToInt(currentTime % 60); // 计算剩余的秒数
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // 更新文本
    }

    void OnCountdownEnd()
    {
        // 倒计时结束后的操作
        Debug.Log("Countdown ended!");
    }
}
