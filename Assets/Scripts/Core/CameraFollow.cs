using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float cameraZ;

    private void Awake()
    {
        cameraZ = transform.position.z;
    }

    private void Start()
    {
        FollowTarget();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, cameraZ);
        }
    }
}
