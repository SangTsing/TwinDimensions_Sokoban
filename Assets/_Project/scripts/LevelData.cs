using UnityEngine; // 必须包含这个，否则不认识 Vector3
using System.Collections.Generic; // 必须包含这个，否则不认识 List

[System.Serializable]
public class LevelItem
{
    public string tag;
    public Vector3 pos;
    public string world; // "Light" 或 "Shadow"
}

[System.Serializable]
public class LevelSaveData
{
    public List<LevelItem> items = new List<LevelItem>();
}