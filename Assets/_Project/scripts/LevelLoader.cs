using UnityEngine;
using System.IO;
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
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string path = Path.Combine(projectRoot, levelFileName + ".json");

        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(json);

        ForceClearAllTaggedObjects();

        GameObject lp = null;
        GameObject sp = null;

        foreach (var item in data.items)
        {
            // 根据记录的 world 属性决定是生成光界还是影界物体
            bool isShadow = (item.world == "Shadow");
            Transform parent = isShadow ? shadowWorldParent : lightWorldParent;

            GameObject obj = InstantiateByTag(item.tag, parent, isShadow);
            if (obj != null)
            {
                obj.transform.position = item.pos; // 放在记录时的绝对位置

                // 寻找并记录玩家对象用于纠缠连线
                if (item.tag == "Player")
                {
                    if (isShadow) sp = obj; else lp = obj;
                }
            }
        }

        // 量子纠缠连线
        if (lp != null && sp != null)
        {
            PlayerController pc = lp.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.shadowPlayer = sp.transform;
                pc.enabled = true;
            }
        }
    }

    private GameObject InstantiateByTag(string tag, Transform parent, bool isShadow)
    {
        GameObject prefab = tag switch
        {
            "Wall" => isShadow ? wallShadow : wallLight,
            "Box" => box, // 箱子和目标点通常两界通用
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