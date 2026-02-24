using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    const string TriggerTag = "BallTrigger";
    const string OutTag = "Out";
    const string RimTag = "Rim";
    const string BackboardTag = "Backboard";

    public enum ShotResult
    {
        Perfect,
        Backboard,
        Normal,
        Miss
    }

    public delegate void OnShotEnd(ShotResult outcome);
    public event OnShotEnd onShotEnd;

    public float inputStrength;
    public float shootAngle = 55f;

    [SerializeField]
    GameObject ballTarget;
    [SerializeField]
    Rigidbody ballRb;
    float gravity = 9.81f;

    Vector3 shootingPosition = Vector3.zero;
    bool touchedRim = false;
    bool touchedBackboard = false;
    bool isShooting = false;
    bool scored = false;

    private void Start()
    {
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    public void Shoot(float power)
    {
        if (!ballRb) return;

        ballRb.velocity = GetBallDirection() * power;
        isShooting = true;
        InputManager.disableInputs = true;
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
        shootingPosition = newPosition;
        ResetPosition();
    }

    public void ResetPosition(bool enableInputs = true)
    {
        if (!ballRb) return;

        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = shootingPosition;
        touchedBackboard = false;
        touchedRim = false;
        scored = false;
        isShooting = false;

        if (enableInputs)
            InputManager.disableInputs = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            scored = true;

            if (touchedBackboard)
            {
                onShotEnd?.Invoke(ShotResult.Backboard);
                return;
            }

            if (touchedRim)
            {
                onShotEnd?.Invoke(ShotResult.Normal);
                return;
            }

            if (!touchedRim && !touchedBackboard)
            {
                onShotEnd?.Invoke(ShotResult.Perfect);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!scored && isShooting && collision.gameObject.tag == OutTag)
        {
            onShotEnd?.Invoke(ShotResult.Miss);
        }

        if (collision.gameObject.tag == RimTag)
        {
            touchedRim = true;
        }

        if (collision.gameObject.tag == BackboardTag)
        {
            touchedBackboard = true;
        }
    }



}
