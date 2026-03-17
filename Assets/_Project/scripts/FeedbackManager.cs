using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening; // 必须确保项目中已导入 DOTween 插件

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;

    [Header("UI 引用")]
    [Tooltip("Canvas 下的全屏红色 Image，用于屏幕红闪反馈")]
    public Image flashImage;

    void Awake()
    {
        // 严谨的单例模式
        if (Instance == null)
        {
            Instance = this;
            // 如果你想让这个管理器跨场景存在，可以取消下面行的注释
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始 UI 状态重置
        if (flashImage != null)
        {
            flashImage.color = Color.clear;
            flashImage.raycastTarget = false; // 确保不会拦截鼠标点击
        }
    }

    /// <summary>
    /// 核心反馈函数：触发光/影主角同步朴实震动以及全屏红闪，并在结束后精准归位
    /// </summary>
    /// <param name="lightPlayer">光界主角的 Transform</param>
    /// <param name="shadowPlayer">影界虚影的 Transform</param>
    public void PlayBlockedFeedback(Transform lightPlayer, Transform shadowPlayer)
    {
        if (lightPlayer == null || shadowPlayer == null) return;

        // 1. 记录原始状态（用于动画结束后的绝对精准对齐）
        Vector3 lOriginalPos = lightPlayer.position;
        Vector3 sOriginalPos = shadowPlayer.position;

        // 2. 执行屏幕红闪（先停止之前的协程防止颜色叠加异常）
        if (flashImage != null)
        {
            StopCoroutine(nameof(FlashScreenRoutine));
            StartCoroutine(nameof(FlashScreenRoutine));
        }

        // 3. 执行物理反馈动画 (DOTween) - 仅保留轻微震动

        // --- 光界主角反馈 ---
        // 使用 OnComplete 协程强制移回原位，确保位置不偏移
        lightPlayer.DOShakePosition(0.1f, 0.1f, 10, 90, false, true) // 震动参数也调小了，更朴实
            .OnComplete(() => StartCoroutine(MoveBack(lightPlayer, lOriginalPos)));

        // --- 影界虚影反馈 ---
        shadowPlayer.DOShakePosition(0.1f, 0.1f, 10, 90, false, true)
            .OnComplete(() => StartCoroutine(MoveBack(shadowPlayer, sOriginalPos)));
    }

    // 强制归位协程：确保浮点数运算后物体依然在格点中心
    private IEnumerator MoveBack(Transform target, Vector3 originalPos)
    {
        float elapsed = 0;
        float duration = 0.05f; // 极短时间内平滑插值回归
        Vector3 currentPos = target.position;

        while (elapsed < duration)
        {
            target.position = Vector3.Lerp(currentPos, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 最终绝对锁定
        target.position = originalPos;
    }

    // 屏幕红闪渐变逻辑（也调得更轻微了）
    private IEnumerator FlashScreenRoutine()
    {
        float duration = 0.2f; // 缩短持续时间
        float maxAlpha = 0.25f; // 降低红色最高时的不透明度，更柔和
        float elapsed = 0f;

        // 初始瞬间变红
        flashImage.color = new Color(1, 0, 0, maxAlpha);

        // 平滑渐隐至透明
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(maxAlpha, 0f, elapsed / duration);
            flashImage.color = new Color(1, 0, 0, currentAlpha);
            yield return null;
        }

        flashImage.color = Color.clear;
    }
}