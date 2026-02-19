using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHandler : MonoBehaviour
{
    const string TriggerTag = "BallTrigger";

    public Vector3 force;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shoot()
    {
        rb.AddForce(force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TriggerTag)
        {
            Debug.Log("Green");
        }
    }
}
