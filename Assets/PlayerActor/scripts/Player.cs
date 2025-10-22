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
        animator = getComponent<animator>();
        controller = getComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
