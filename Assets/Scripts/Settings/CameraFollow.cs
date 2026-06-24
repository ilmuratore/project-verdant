using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void Awake()
    {
        ResolveTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            ResolveTarget();
        }

        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    private void ResolveTarget()
    {
        if (target != null && target.gameObject.scene.IsValid()) return;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            target = player.transform;
        }
    }
}
