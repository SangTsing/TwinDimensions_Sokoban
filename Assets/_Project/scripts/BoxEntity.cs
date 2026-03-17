using UnityEngine;
using System.Collections;

public class BoxEntity : MonoBehaviour
{
    private bool isMoving = false;

    public void Push(Vector3 direction, float duration)
    {
        StartCoroutine(DoMove(transform.position + direction, duration));
    }

    IEnumerator DoMove(Vector3 target, float duration)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        isMoving = false;
    }
}