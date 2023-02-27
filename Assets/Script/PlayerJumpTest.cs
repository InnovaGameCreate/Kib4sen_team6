using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerJumpTest : MonoBehaviour
{
    private Rigidbody rb;
    private int UpForce;
    private float Distance;
    private Animator animator;
    private bool isJump;
    private bool isGetGround;

    // 音声関連
    private AudioSource jumpUpSound; // ジャンプ開始時SE
    private AudioSource jumpDownSound;     // ジャンプ終了時SE
    [SerializeField] float pitchRange = 0.1f; // ピッチのランダム幅
    //

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpForce = 300;
        Distance = 0.3f;
        animator = GetComponent<Animator>();
        isJump = false;
        isGetGround = false;
        // 音声データの取得
        jumpUpSound = GetComponents<AudioSource>()[2];
        jumpDownSound = GetComponents<AudioSource>()[3];
        //
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.down);
        bool isGround = Physics.Raycast(ray, Distance);
        animator.SetBool("isGround", isGround);
        Debug.DrawRay(rayPosition, Vector3.down * Distance, Color.red);

        
        if (Input.GetKeyDown("space") && isGround && !animator.IsInTransition(0) && (animator.GetCurrentAnimatorStateInfo(0).IsName("Standing@loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop")))
        {
            animator.SetBool("Jump", true);
            isJump = true;
        }
        if(isJump == true && animator.GetCurrentAnimatorStateInfo(0).IsName("JumpToTop"))
        {
            rb.AddForce(new Vector3(0, UpForce, 0));
            PlayJumpUpSound(); // 離陸音声の再生
            isJump = false;
            isGetGround = true;
        }
        if(isGetGround == true && animator.GetCurrentAnimatorStateInfo(0).IsName("TopToGround"))
        {
            PlayJumpDownSound(); // 着地音声の再生
            isGetGround = false;
        }
        if (!isGround || !Input.GetKeyDown("space"))
        {
            animator.SetBool("Jump", false);
        }

    }

    // 音声を再生する関数
    private void PlayJumpUpSound()
    {
        jumpUpSound.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        jumpUpSound.PlayOneShot(jumpUpSound.clip);
    }
    private void PlayJumpDownSound()
    {
        jumpDownSound.pitch = 0.75f + Random.Range(-pitchRange, pitchRange);
        jumpDownSound.PlayOneShot(jumpDownSound.clip);
    }
}
