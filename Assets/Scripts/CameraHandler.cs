using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float cameraBackOffset;
    public float cameraUpOffset;
    public float cameraSpeed = 1.5f;

    [SerializeField]
    GameObject ball;
    [SerializeField]
    GameObject backgroundObject;
    [SerializeField]
    GameObject lookAtObject;

    private void Start()
    {
        if (ball == null || backgroundObject == null) return;

        transform.position = GetCameraPosition();
    }

    private void Update()
    {
        if (ball == null || backgroundObject == null) return;

        transform.position = Vector3.Lerp(transform.position, GetCameraPosition(), cameraSpeed * Time.deltaTime);

        if (lookAtObject == null) return;
        
        transform.LookAt(lookAtObject.transform.position);
    }

    Vector3 GetCameraPosition()
    {
        Vector3 flatBackgroundObjectPos = backgroundObject.transform.position;
        flatBackgroundObjectPos.y = 0;
        Vector3 flatBallPos = ball.transform.position;
        flatBallPos.y = 0;

        Vector3 direction = (flatBackgroundObjectPos - flatBallPos).normalized;

        Vector3 final = ball.transform.position - direction * cameraBackOffset + Vector3.up * cameraUpOffset;

        return final;
    }
}
