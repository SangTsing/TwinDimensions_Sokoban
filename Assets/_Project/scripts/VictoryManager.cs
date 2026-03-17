using UnityEngine;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;
    public GameObject victoryPanel; // 胜利弹窗
    private LevelLoader loader;

    void Awake() => Instance = this;

    void Start()
    {
        loader = FindObjectOfType<LevelLoader>();
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        // 快捷键 R 重置
        if (Input.GetKeyDown(KeyCode.R)) ResetLevel();
    }

    public void ResetLevel()
    {
        if (loader != null)
        {
            if (victoryPanel != null) victoryPanel.SetActive(false);
            loader.LoadLevel(); // 重新加载当前关卡
        }
    }

    // 点击“下一关”按钮调用此函数
    public void LoadNextLevel()
    {
        if (loader == null) return;

        // 1. 获取当前关卡数字，例如 "Level_1" -> 1
        string currentName = loader.levelFileName;
        if (currentName.Contains("_"))
        {
            string[] parts = currentName.Split('_');
            if (int.TryParse(parts[1], out int num))
            {
                // 2. 这里的 3 是你总共的关卡数
                int nextNum = num + 1;
                if (nextNum > 3)
                {
                    Debug.Log("?? 所有关卡已完成！");
                    // 这里可以跳转回主菜单
                    return;
                }

                // 3. 设置新文件名并加载
                loader.levelFileName = "Level_" + nextNum;
                loader.LoadLevel();

                // 4. 隐藏胜利面板
                if (victoryPanel != null) victoryPanel.SetActive(false);
            }
        }
    }

    public void CheckWinCondition()
    {
        // 获取场景中所有的目标点
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        if (targets.Length == 0) return;

        int reachedCount = 0;
        foreach (var t in targets)
        {
            // 检查目标点位置是否有箱子（利用 Physics.CheckSphere 或 GridManager）
            if (IsBoxOnTarget(t.transform.position)) reachedCount++;
        }

        if (reachedCount >= targets.Length)
        {
            victoryPanel.SetActive(true);
        }
    }

    private bool IsBoxOnTarget(Vector3 pos)
    {
        // 简单的碰撞检测，看位置上是否有 Box 标签的物体
        Collider[] colliders = Physics.OverlapSphere(pos, 0.1f);
        foreach (var c in colliders)
        {
            if (c.CompareTag("Box")) return true;
        }
        return false;
    }
}