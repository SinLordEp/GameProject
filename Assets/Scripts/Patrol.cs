using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header("Patrol Border")]
    public Transform leftBoundary;     // 左边界
    public Transform rightBoundary;    // 右边界

    [Header("Patrol stats")]
    public float speed = 2f;                // 移动速度
    public float waitTimeAtBoundary = 1f;   // 到达边界后等待时间

    private Vector3 targetPosition;         // 当前目标位置
    private bool isWaiting = false;         // 是否正在等待
    private float waitTimer = 0f;           // 等待计时器

    void Start()
    {
        float centerX = (leftBoundary.position.x + rightBoundary.position.x) / 2;
        if (transform.position.x < centerX)
        {
            targetPosition = rightBoundary.position;
        }
        else
        {
            targetPosition = leftBoundary.position;
        }
    }

    void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
            }
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isWaiting = true;
            waitTimer = waitTimeAtBoundary;
            targetPosition = (targetPosition == leftBoundary.position) ? rightBoundary.position : leftBoundary.position;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }
}
