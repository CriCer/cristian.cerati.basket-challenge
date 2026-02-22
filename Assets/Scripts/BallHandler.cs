using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    const string TriggerTag = "BallTrigger";



    public float inputStrength;
    public float shootAngle = 55f;

    [SerializeField]
    GameObject ballTarget;

    Rigidbody rb;
    float gravity = 9.81f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    public void Shoot(float power)
    {
        rb.velocity = GetBallDirection() * power;
    }

    private Vector3 GetBallDirection()
    {
        Vector3 flatBallPosition = transform.position;
        flatBallPosition.y = 0;
        Vector3 flatTargetPosition = ballTarget.transform.position;
        flatTargetPosition.y = 0;

        Vector3 flatDirection = (flatTargetPosition - flatBallPosition).normalized;

        Quaternion tilt = Quaternion.AngleAxis(shootAngle, Vector3.Cross(flatDirection, Vector3.up));
        Vector3 finalDir = tilt * flatDirection;


        return finalDir;
    }

    public float GetPerfectShotRequiredPower()
    {
        Vector3 flatBallPosition = transform.position;
        flatBallPosition.y = 0;
        Vector3 flatTargetPosition = ballTarget.transform.position;
        flatTargetPosition.y = 0;

        float y = ballTarget.transform.position.y - transform.position.y;
        float x = (flatTargetPosition - flatBallPosition).magnitude;

        float angleRad = shootAngle * Mathf.Deg2Rad;

        float v = Mathf.Sqrt((gravity * x * x) / (2f * Mathf.Pow(Mathf.Cos(angleRad), 2) * (x * Mathf.Tan(angleRad) - y)));

        if (float.IsNaN(v))
        {
            return 1;
        }

        return v;
    }

    public void NewPosition(Vector3 newPosition)
    {

    }

    public void ResetPosition()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            Debug.Log("Green");
        }
    }

    

}
