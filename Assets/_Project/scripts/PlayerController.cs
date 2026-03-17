using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform shadowPlayer;
    public float moveDuration = 0.2f;
    private bool isMoving = false;

    void Update()
    {
        // 安全锁：如果影身没连上，或者还在动，或者刚加载完0.1秒内，不响应输入
        if (shadowPlayer == null || isMoving || Time.timeSinceLevelLoad < 0.1f) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            Vector3 dir = (h != 0) ? new Vector3(h, 0, 0) : new Vector3(0, 0, v);
            TryMove(dir);
        }
    }

    private void TryMove(Vector3 dir)
    {
        if (shadowPlayer == null) return; // 二重保险

        Vector3 lTarget = transform.position + dir;
        Vector3 sTarget = shadowPlayer.position + dir;

        // 探测逻辑... (省略中间探测代码，保持你原来的即可)
        // 判定 canMove...
        bool canMove = ValidateMove(dir, lTarget, sTarget);

        if (canMove) StartCoroutine(DoSyncMove(lTarget, sTarget));
        else TriggerFeedback();
    }

    private bool ValidateMove(Vector3 dir, Vector3 lt, Vector3 st)
    {
        BoxEntity lb = GridManager.GetBoxAt(lt);
        BoxEntity sb = GridManager.GetBoxAt(st);
        if (lb == null && sb == null) return !GridManager.IsWallAt(lt) && !GridManager.IsWallAt(st);

        bool lp = (lb == null) ? !GridManager.IsWallAt(lt) : GridManager.IsPositionOpen(lt + dir);
        bool sp = (sb == null) ? !GridManager.IsWallAt(st) : GridManager.IsPositionOpen(st + dir);
        if (lp && sp)
        {
            if (lb != null) lb.Push(dir, moveDuration);
            if (sb != null) sb.Push(dir, moveDuration);
            return true;
        }
        return false;
    }

    IEnumerator DoSyncMove(Vector3 lt, Vector3 st)
    {
        isMoving = true;
        Vector3 ls = transform.position; Vector3 ss = shadowPlayer.position;
        float e = 0;
        while (e < moveDuration)
        {
            float t = e / moveDuration;
            transform.position = Vector3.Lerp(ls, lt, t);
            shadowPlayer.position = Vector3.Lerp(ss, st, t);
            e += Time.deltaTime; yield return null;
        }
        transform.position = lt; shadowPlayer.position = st;
        if (VictoryManager.Instance != null) VictoryManager.Instance.CheckWinCondition();
        isMoving = false;
    }

    void TriggerFeedback()
    {
        if (FeedbackManager.Instance != null) FeedbackManager.Instance.PlayBlockedFeedback(transform, shadowPlayer);
    }
}