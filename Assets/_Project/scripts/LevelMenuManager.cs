using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    public static LevelMenuManager Instance;

    [Header("UI 面板引用")]
    [Tooltip("选关主菜单面板")]
    public GameObject menuPanel;

    private LevelLoader loader;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        loader = FindObjectOfType<LevelLoader>();

        // 游戏启动时默认显示选关菜单
        ShowMenu();
    }

    /// <summary>
    /// 核心功能：由选关按钮调用，参数为关卡数字 (1, 2, 3)
    /// </summary>
    /// <param name="levelIndex">关卡编号</param>
    public void SelectLevel(int levelIndex)
    {
        if (loader == null) loader = FindObjectOfType<LevelLoader>();

        // 1. 设置加载器文件名并执行加载
        loader.levelFileName = "Level_" + levelIndex;
        loader.LoadLevel();

        // 2. 关闭选关菜单
        HideMenu();

        // 3. 确保胜利面板是关闭的（防止从胜利状态直接切关卡导致 UI 残留）
        if (VictoryManager.Instance != null && VictoryManager.Instance.victoryPanel != null)
        {
            VictoryManager.Instance.victoryPanel.SetActive(false);
        }

        Debug.Log($"<color=cyan>成功加载关卡：Level_{levelIndex}</color>");
    }

    /// <summary>
    /// 显示选关菜单并清理当前场景
    /// </summary>
    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);

        // 返回菜单时清理掉场景中现有的物体，保持背景整洁
        if (loader != null)
        {
            // 这里调用我们之前在 LevelLoader 里写的清理逻辑
            // 如果你的 LevelLoader 清理函数是私有的，请将其改为 public
            loader.LoadLevel(); // 或者直接调用清理函数，如果只想清空建议单独提个方法
        }
    }

    /// <summary>
    /// 隐藏选关菜单
    /// </summary>
    public void HideMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
    }
}