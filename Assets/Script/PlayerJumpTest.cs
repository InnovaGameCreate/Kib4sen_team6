using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpTest : MonoBehaviour
{
    private Rigidbody rb;
    private int UpForce;
    private float Distance;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpForce = 200;
        Distance = 0.3f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.down);
        bool isGround = Physics.Raycast(ray, Distance);
        animator.SetBool("isGround", isGround);
        Debug.DrawRay(rayPosition, Vector3.down * Distance, Color.red);

        
        if (Input.GetKeyDown("space"))
        {
            if (isGround && (animator.GetCurrentAnimatorStateInfo(0).IsName("Standing@loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop")))
            {
                rb.AddForce(new Vector3(0, UpForce, 0));
                animator.SetBool("Jump", true);
            }
        }
        if (!isGround)
        {
            animator.SetBool("Jump", false);
        }

    }
}
