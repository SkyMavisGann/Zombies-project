using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float baseSpeed;
    public float runAmp;

    public bool lcokCursor;

    private CharacterController controller;
    private Animator animator;

    private float airTime;
    private float gravity = -9.8f;

    private PlayerMovementInfo info;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        info = new PlayerMovementInfo();
        info.baseSpeed = baseSpeed;
        info.runAmp = runAmp;

        if (lcokCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        airTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();

        BlendAnimation();

        CalculateVec();

        doMove();

        Rotate();
    }

    public void ProcessInput()
    {
        info.lnr = Input.GetAxis("Horizontal");
        info.fnb = Input.GetAxis("Vertical");

        info.moving = (info.fnb != 0) || (info.lnr != 0);
        if (info.moving)
        {
            info.movingFw = info.fnb > 0;
            info.movingBk = info.fnb < 0;
        }

        bool sprinting = (info.moving && !info.movingBk && Input.GetKey(KeyCode.LeftShift));

        if (sprinting)
        {
            info.speed = info.baseSpeed * info.runAmp;
        }
        else
        {
            info.speed = info.baseSpeed;
            info.fnb /= 2.0f;
        }
    }

    public void BlendAnimation()
    {
        animator.SetFloat("LnR", info.lnr);
        animator.SetFloat("FnB", info.fnb);
    }

    public void CalculateVec()
    {
        Vector3 moveDirFw = transform.forward * info.fnb;
        Vector3 moveDirSide = transform.right * info.lnr;

        info.dir = moveDirFw + moveDirSide;
        info.normDir = info.dir.normalized;

        info.dist = info.normDir * info.speed * Time.deltaTime;

        GroundPlayer();
    }

    public void doMove()
    {
        controller.Move(info.dist);
    }

    public void GroundPlayer()
    {
        if (controller.isGrounded)
        {
            airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;

            Vector3 dir = info.normDir;
            dir.y += 0.5f * gravity * airTime;

            info.normDir = dir;

            info.dist = info.normDir * airTime;
        }
    }

    public void Rotate()
    {
        Vector3 theta;

        if (Input.GetKey(KeyCode.T))
        {
            return;
        }
        else
        {
            theta = Camera.main.transform.eulerAngles;
            theta.x = 0;
            theta.z = 0;

            transform.eulerAngles = theta;
        }
    }
                           
}
