using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    public string levelFileName = "Level_1";
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
        // --- 核心修改：使用 Resources.Load 代替 System.IO ---
        // 注意：Resources.Load 不需要文件后缀名
        TextAsset jsonFile = Resources.Load<TextAsset>(levelFileName);

        if (jsonFile == null)
        {
            Debug.LogError($"? 无法在 Resources 文件夹中找到关卡: {levelFileName}");
            return;
        }

        string json = jsonFile.text;
        LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(json);

        ForceClearAllTaggedObjects();

        Vector3 worldOffset = Vector3.zero;
        if (lightWorldParent != null && shadowWorldParent != null)
        {
            worldOffset = shadowWorldParent.position - lightWorldParent.position;
        }

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

        if (lp != null && sp != null)
        {
            PlayerController pc = lp.GetComponent<PlayerController>();
            if (pc != null) { pc.shadowPlayer = sp.transform; pc.enabled = true; }
        }
        Debug.Log($"<color=cyan>? 关卡 {levelFileName} 加载成功！</color>");
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

    private void ForceClearAllTaggedObjects()
    {
        string[] tags = { "Wall", "Box", "Target", "Player" };
        foreach (string t in tags)
        {
            foreach (GameObject o in GameObject.FindGameObjectsWithTag(t))
            {
                if (Application.isPlaying) Destroy(o);
                else DestroyImmediate(o);
            }
        }
    }
}