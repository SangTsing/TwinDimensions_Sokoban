using UnityEngine;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    public GameObject victoryPanel;
    public Text victoryText; // 可选：用于显示“全关通关”
    private LevelLoader loader;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        loader = FindObjectOfType<LevelLoader>();
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    void Update()
    {
        // WebGL 焦点提醒：必须点一下画面，按键才生效
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCurrentLevel();
        }
    }

    // 由 PlayerController 在推完箱子后调用
    public void CheckWinCondition()
    {
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        if (boxes.Length == 0) return;

        int onTargetCount = 0;
        foreach (var b in boxes)
        {
            foreach (var t in targets)
            {
                if (Vector3.Distance(b.transform.position, t.transform.position) < 0.1f)
                {
                    onTargetCount++;
                    break;
                }
            }
        }

        if (onTargetCount >= boxes.Length && boxes.Length > 0)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
    }

    // 核心改进：自动跳转到下一关
    public void LoadNextLevelAuto()
    {
        string currentName = loader.levelFileName;
        string[] parts = currentName.Split('_');

        if (parts.Length >= 2 && int.TryParse(parts[1], out int currentNum))
        {
            int nextNum = currentNum + 1;
            string nextName = "Level_" + nextNum;

            // 尝试预览加载，检查是否存在下一关
            if (Resources.Load<TextAsset>(nextName) != null)
            {
                loader.levelFileName = nextName;
                if (victoryPanel != null) victoryPanel.SetActive(false);
                loader.LoadLevel();
            }
            else
            {
                // 如果没有 Level_4 了，显示通关信息
                if (victoryText != null) victoryText.text = "All Levels Cleared!";
                Debug.Log("?? 已完成所有关卡！");
                // 3秒后自动回菜单
                Invoke("BackToMenuDelayed", 2f);
            }
        }
    }

    private void BackToMenuDelayed()
    {
        FindObjectOfType<LevelMenuManager>().BackToMenu();
    }

    public void ResetCurrentLevel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        loader.LoadLevel();
    }
}