using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    public LevelLoader loader;
    public GameObject menuPanel;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    // 关卡按钮调用 (1, 2, 3)
    public void SelectLevel(int levelNum)
    {
        loader.levelFileName = "Level_" + levelNum;
        if (menuPanel != null) menuPanel.SetActive(false);
        loader.LoadLevel();
    }

    // “返回菜单”按钮调用
    public void BackToMenu()
    {
        // 1. 显示菜单
        if (menuPanel != null) menuPanel.SetActive(true);

        // 2. 隐藏胜利面板
        VictoryManager.Instance.victoryPanel.SetActive(false);

        // 3. 清空当前场景物体
        loader.ForceClearAllTaggedObjects();
    }
}