using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("当前加载的关卡名")]
    public string levelFileName = "Level_1";

    [Header("世界根节点")]
    public Transform lightWorldParent;
    public Transform shadowWorldParent;

    [Header("预制体库")]
    public GameObject wallLight;
    public GameObject wallShadow;
    public GameObject box;
    public GameObject target;
    public GameObject player;
    public GameObject shadowPlayerPrefab;

    public void LoadLevel()
    {
        // 1. 彻底清除旧关卡物体
        ForceClearAllTaggedObjects();

        // 2. 自动修正路径：去掉空格和后缀
        string cleanPath = levelFileName.Replace(".json", "").Trim();

        // 3. 从 Assets/Resources 加载
        TextAsset jsonFile = Resources.Load<TextAsset>(cleanPath);

        if (jsonFile == null)
        {
            Debug.LogError($"<color=red>? [资源缺失]</color> 找不到文件: Assets/Resources/{cleanPath}.json");
            return;
        }

        LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(jsonFile.text);

        GameObject lp = null; GameObject sp = null;

        foreach (var item in data.items)
        {
            bool isShadow = (item.world == "Shadow");
            Transform parent = isShadow ? shadowWorldParent : lightWorldParent;

            GameObject obj = InstantiateByTag(item.tag, parent, isShadow);
            if (obj != null)
            {
                obj.transform.position = item.pos;
                if (item.tag == "Player")
                {
                    if (isShadow) sp = obj; else lp = obj;
                }
            }
        }

        // 4. 初始化双界主角逻辑
        if (lp != null && sp != null)
        {
            PlayerController pc = lp.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.shadowPlayer = sp.transform;
                pc.enabled = true;
            }
        }
        Debug.Log($"<color=cyan>? 关卡 {cleanPath} 加载成功！</color>");
    }

    private GameObject InstantiateByTag(string tag, Transform parent, bool isShadow)
    {
        GameObject prefab = tag switch
        {
            "Wall" => isShadow ? wallShadow : wallLight,
            "Box" => box,
            "Target" => target,
            "Player" => isShadow ? shadowPlayerPrefab : player,
            _ => null
        };
        return prefab != null ? Instantiate(prefab, parent) : null;
    }

    // 公开此方法，方便菜单脚本调用清场
    public void ForceClearAllTaggedObjects()
    {
        string[] tags = { "Wall", "Box", "Target", "Player" };
        foreach (string t in tags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(t);
            foreach (GameObject o in objs)
            {
                if (Application.isPlaying) Destroy(o);
                else DestroyImmediate(o);
            }
        }
    }
}