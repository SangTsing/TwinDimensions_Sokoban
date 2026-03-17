using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LevelExporter : MonoBehaviour
{
    public string fileName = "Level_1";
    public float xThreshold = 15f; // 这里的界限用来区分左右世界

    [ContextMenu("Export Level to JSON")]
    public void Export()
    {
        LevelSaveData data = new LevelSaveData();
        string[] targetTags = { "Wall", "Box", "Target", "Player" };

        foreach (string tag in targetTags)
        {
            GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in foundObjects)
            {
                // 判断属于哪个世界
                string belongWorld = (obj.transform.position.x < xThreshold) ? "Light" : "Shadow";

                data.items.Add(new LevelItem
                {
                    tag = obj.tag,
                    pos = obj.transform.position, // 记录绝对世界坐标
                    world = belongWorld
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string path = Path.Combine(Application.dataPath, "Resources", fileName + ".json");
        File.WriteAllText(path, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log($"<color=green>?? 独立布局导出成功！共保存 {data.items.Count} 个物体。</color>");
    }
}