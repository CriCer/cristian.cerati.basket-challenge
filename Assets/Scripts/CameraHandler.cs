using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public float cameraBackOffset;
    public float cameraUpOffset;
    public float cameraSmoothingTime = 0.5f;

    [SerializeField]
    GameObject ball;
    [SerializeField]
    GameObject backgroundObject;
    [SerializeField]
    GameObject lookAtObject;

    Vector3 currentVelocity = Vector3.zero;


    private void Start()
    {
        if (ball == null || backgroundObject == null) return;

        transform.position = GetCameraPosition();
    }

    private void Update()
    {
        if (ball == null || backgroundObject == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, GetCameraPosition(), ref currentVelocity, cameraSmoothingTime);

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
