using UnityEngine;

public class TargetCube : Target
{
    bool rotationOn = false;
    float rotationAmount = 360f;
    float currentRotationAmount = 0f;

    Vector3 rotation;

    float time;


    void Update()
    {
        if (rotationOn)
        {
            if (currentRotationAmount < rotationAmount)
            {
                time = Time.deltaTime;
                target.transform.Rotate(rotation * time);
                currentRotationAmount += rotationAmount * time;
            }
            else // finished
            {
                rotationOn = false;
                target.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    void Rotate (float degrees)
    {
        if (!rotationOn)
        {
            rotationOn = true;
            rotationAmount = degrees;
            currentRotationAmount = 0f;
            rotation = new Vector3(0, degrees, 0);
        }
    }

    public override void Process(RaycastHit hit)
    {
        if (gameObject.tag == "Target")
        {
            Rotate(rotationAmount);
        }

        effectScript.Play(hit, hitSound, hitEffect, effectDuration);
    }
}
