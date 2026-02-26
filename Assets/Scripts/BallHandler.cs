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
    public float normalShootAngle = 80f;
    public float backboardShootAngle = 55f;

    public GameObject ballTarget;

    [SerializeField]
    GameObject backboard;
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

    #region Public methods
    public void Shoot(float power)
    {
        if (!ballRb || !ballTarget) return;

        if (power == GetBackboardShotRequiredPower())
        {
            Vector3 reflectedPoint = GetMirroredPosition(ballTarget.transform.position, backboard.transform.position, backboard.transform.right);
            ballRb.velocity = GetBallDirection(reflectedPoint, backboardShootAngle) * power;
        }
        else
        {
            ballRb.velocity = GetBallDirection(ballTarget.transform.position, normalShootAngle) * power;
        }

        isShooting = true;
        InputManager.disableInputs = true;
    }

    public float GetPerfectShotRequiredPower()
    {
        return GetShotRequiredPower(ballTarget.transform.position, normalShootAngle);
    }

    public float GetBackboardShotRequiredPower()
    {
        Vector3 reflectedPoint = GetMirroredPosition(ballTarget.transform.position, backboard.transform.position, backboard.transform.right);
        return GetShotRequiredPower(reflectedPoint, backboardShootAngle);
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
#endregion

#region Private methods
    Vector3 GetBallDirection(Vector3 target, float angle)
    {
        Vector3 flatBallPosition = transform.position;
        flatBallPosition.y = 0;
        Vector3 flatTargetPosition = target;
        flatTargetPosition.y = 0;

        Vector3 flatDirection = (flatTargetPosition - flatBallPosition).normalized;

        Quaternion tilt = Quaternion.AngleAxis(angle, Vector3.Cross(flatDirection, Vector3.up));
        Vector3 finalDir = tilt * flatDirection;


        return finalDir;
    }

    float GetShotRequiredPower(Vector3 target, float angle)
    {
        Vector3 flatBallPosition = transform.position;
        flatBallPosition.y = 0;
        Vector3 flatTargetPosition = target;
        flatTargetPosition.y = 0;

        float y = target.y - transform.position.y;
        float x = (flatTargetPosition - flatBallPosition).magnitude;

        float angleRad = angle * Mathf.Deg2Rad;

        float v = Mathf.Sqrt((gravity * x * x) / (2f * Mathf.Pow(Mathf.Cos(angleRad), 2) * (x * Mathf.Tan(angleRad) - y)));

        if (float.IsNaN(v))
        {
            return 1;
        }

        return v;
    }

    Vector3 GetMirroredPosition(Vector3 target, Vector3 mirror, Vector3 mirrorAxis)
    {
        mirrorAxis = mirrorAxis.normalized;
        Vector3 offset = target - mirror;
        Vector3 projected = Vector3.Project(offset, mirrorAxis);
        Vector3 mirrored = target - 2f * projected;
        return mirrored;
    }
#endregion

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
