using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class QuickLevelBrush : Editor
{
    // --- 路径配置区 ---
    private static string wallLightPath = "Assets/_Project/prefabs/Wall_LIGHT.prefab";
    private static string wallShadowPath = "Assets/_Project/prefabs/Wall_SHADOW.prefab";
    private static string boxPath = "Assets/_Project/Prefabs/Box.prefab";
    private static string targetPath = "Assets/_Project/prefabs/Target Pad.prefab"; // 请确认此路径

    // --- 状态记录 ---
    private static bool isBPressed = false;
    private static bool isTPressed = false;

    static QuickLevelBrush()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // 1. 监听键盘按下与抬起
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.B) isBPressed = true;
            if (e.keyCode == KeyCode.T) isTPressed = true;
        }
        if (e.type == EventType.KeyUp)
        {
            if (e.keyCode == KeyCode.B) isBPressed = false;
            if (e.keyCode == KeyCode.T) isTPressed = false;
        }

        // 2. 监听鼠标点击
        if (e.type == EventType.MouseDown)
        {
            // Shift + 左键 -> 光界墙
            if (e.shift && e.button == 0)
            {
                PlaceObject(wallLightPath);
                e.Use();
            }
            // Shift + 右键 -> 影界墙
            else if (e.shift && e.button == 1)
            {
                PlaceObject(wallShadowPath);
                e.Use();
            }
            // B + 左键 -> 箱子
            else if (isBPressed && e.button == 0)
            {
                PlaceObject(boxPath);
                e.Use();
            }
            // T + 左键 -> 目标点 (Target Pad)
            else if (isTPressed && e.button == 0)
            {
                PlaceObject(targetPath);
                e.Use();
            }
        }
    }

    private static void PlaceObject(string path)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            // 1x1 网格对齐逻辑
            Vector3 snappedPos = new Vector3(
                Mathf.Floor(hitPoint.x) + 0.5f,
                0,
                Mathf.Floor(hitPoint.z) + 0.5f
            );

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                GameObject parent = GameObject.Find("--LIGHT_WORLD--");

                // 防重复检测
                if (IsPositionOccupied(parent, snappedPos)) return;

                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                newObj.transform.SetParent(parent?.transform);
                newObj.transform.localPosition = snappedPos;

                Undo.RegisterCreatedObjectUndo(newObj, "Brush Place");
            }
            else
            {
                Debug.LogWarning($"[QuickBrush] 找不到预制体，请检查路径: {path}");
            }
        }
    }

    private static bool IsPositionOccupied(GameObject parent, Vector3 localPos)
    {
        if (parent == null) return false;
        foreach (Transform child in parent.transform)
        {
            if (Vector3.Distance(child.localPosition, localPos) < 0.1f) return true;
        }
        return false;
    }
}