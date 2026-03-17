using UnityEngine;

public class TargetPad : MonoBehaviour
{
    public bool isOccupied = false;

    // 当有物体进入触发区
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            // 检查箱子是否基本对齐了踏板中心
            if (Vector3.Distance(transform.position, other.transform.position) < 0.2f)
            {
                isOccupied = true;
            }
        }
    }

    // 当物体离开触发区
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            isOccupied = false;
        }
    }
}