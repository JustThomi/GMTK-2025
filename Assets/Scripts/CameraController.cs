using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float followSpeed = 5f;
    public float rotateSpeed = 5f;

    public float distance = 6f;
    public float height = 2f;
    public float pitchAngle = 10f;

    private Vector3 currentVelocity;

    private void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(pitchAngle, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    public void SetCameraDistanceAndHeight(float newDistance, float newHeight)
    {
        distance = Mathf.Max(0.1f, newDistance);
        height = Mathf.Max(0f, newHeight);
    }
}
