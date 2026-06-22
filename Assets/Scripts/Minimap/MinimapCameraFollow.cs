using UnityEngine;

public class MinimapCameraFollowe : MonoBehaviour
{
    [Header("Target da seguire")]
    [SerializeField] private Transform target;

    [Header("Impostazioni camera")]
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
            );

    }
}
