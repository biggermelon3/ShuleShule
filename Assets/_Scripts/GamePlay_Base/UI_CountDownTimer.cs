using System.Collections;
using UnityEngine;
using TMPro;

public class UI_CountDownTimer : MonoBehaviour
{
    /*deprecated
    [Header("Countdown Settings")]
    [Tooltip("Countdown time in seconds")]
    public float countdownTime = 60f; // 倒计时时间，以秒为单位

     // 用于显示倒计时的 TextMeshPro 组件


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

    

    void OnCountdownEnd()
    {
        // 倒计时结束后的操作
        EventManager.GameOver.Invoke();
        Debug.Log("Countdown ended!");
    }
    */
}
