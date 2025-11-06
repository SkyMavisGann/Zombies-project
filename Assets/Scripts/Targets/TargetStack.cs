using System.Diagnostics;
using UnityEngine;

public class TargetStack : Target
{
    public float impactForce;

    Rigidbody targetRigidbody;
    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    public override void Process(RaycastHit hit)
    {
        targetRigidbody.AddForce(-hit.normal * impactForce);

        effectScript.Play(hit, hitSound, hitEffect, effectDuration);

    }
}
