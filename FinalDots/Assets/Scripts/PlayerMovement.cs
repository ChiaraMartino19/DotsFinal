using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] bool useCameraForward = true;

    Camera cachedCam;

    void Awake()
    {
        cachedCam = Camera.main;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir;

        if (useCameraForward && cachedCam != null)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(cachedCam.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cachedCam.transform.right, Vector3.up).normalized;

            dir = camRight * h + camForward * v;
        }
        else
        {
            dir = new Vector3(h, 0f, v);
        }

        if (dir.sqrMagnitude > 1f) dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
