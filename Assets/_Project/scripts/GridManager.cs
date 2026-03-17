using UnityEngine;

public static class GridManager
{
    // 检测目标位置是否可以通行（没有墙且没有箱子）
    public static bool IsPositionOpen(Vector3 targetPos)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPos, 0.2f);
        foreach (var col in colliders)
        {
            // 如果有墙或箱子，该格子就不是“空”的
            if (col.CompareTag("Wall") || col.CompareTag("Box"))
            {
                return false;
            }
        }
        return true;
    }

    // 专门检测是否有墙
    public static bool IsWallAt(Vector3 targetPos)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPos, 0.2f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Wall")) return true;
        }
        return false;
    }

    // 获取指定位置的箱子脚本
    public static BoxEntity GetBoxAt(Vector3 targetPos)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPos, 0.2f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Box"))
            {
                return col.GetComponent<BoxEntity>();
            }
        }
        return null;
    }
}