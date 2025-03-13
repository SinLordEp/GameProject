using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Asignaremos al protagonista en la caja de Component
    public Vector3 offset = new Vector3(0, 0, -5);
    public float smoothSpeed = 5f;

    void Start()
    {
        Invoke("FindPlayer", 1f);
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
    void LateUpdate()
    {
        if (target == null) return; // Si no hay target, no hacer nada
        Vector3 desiredPosition = target.position + offset;
       	transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
} 