using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementInfo
{
    public float fnb = 0.0f;
    public float lnr = 0.0f;

    public float speed = 0.0f;
    public Vector3 dir = Vector3.zero;
    public Vector3 normDir = Vector3.zero;
    public Vector3 dist = Vector3.zero;

    public bool movingFw = false;
    public bool movingBk = false;
    public bool moving = false;

    public float baseSpeed;
    public float runAmp;

    //check
}
