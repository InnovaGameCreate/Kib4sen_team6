using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController_Test : MonoBehaviour
{
    private Rigidbody myrigidbody;
    [SerializeField]
    private GameObject ballPrefab;
    Vector2 displayCenter;
    private Transform pos;
    private GameObject block;
    const int DOWN = 0;
    int Remaining = 10;
    private Animator animator;
    private float Distance;
    private bool isGathering = false;
    GameObject Bullet;

    // 音声関連
    private AudioSource gatheringSound; // 雪玉を集めるSE
    private AudioSource throwSound;     // 雪玉を投げるSE
    [SerializeField] float pitchRange = 0.1f; // ピッチのランダム幅
    //

    [SerializeField] private GameObject[] YukidamaUI;
    [SerializeField] private float YukidamaDeleteTime; //焚火に接触したときに手持ち雪玉が減少していく時間の間隔
    private float ytime;

    // Start is called before the first frame update
    void Start()
    {
        displayCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        animator = GetComponent<Animator>();
        Distance = 0.3f;
        
        // 音声関連
        gatheringSound = GetComponents<AudioSource>()[0];
        throwSound = GetComponents<AudioSource>()[1];
        //
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        //条件が分かり次第順次実装
        /*if(Input.GetKey(KeyCode.P))
        {
            animator.SetBool("Down", true);
        }
        if(Input.GetKeyUp(KeyCode.P))
        {
            animator.SetBool("Down", false);
        }
        */
    }
    private void PlayerMove()
    {
        if (Input.GetMouseButtonDown(0) && !animator.IsInTransition(0) && !animator.GetCurrentAnimatorStateInfo(0).IsName("Throw") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Gathering") && !animator.GetCurrentAnimatorStateInfo(0).IsName("GatherFinish") && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpToTop") && !animator.GetCurrentAnimatorStateInfo(0).IsName("TopOfJump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("TopToGround")/* && !animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop")*/)
        {
            if (Remaining > 0)
            {
                ShotSnowBall();
            }
        }
        if (Input.GetMouseButton(1) && !animator.IsInTransition(0))
        {
            ShotRay();
            Debug.Log("bo");
        }
        if (Input.GetMouseButtonUp(1) || Remaining == 10)
        {
            animator.SetBool("Gather", false);
        }
    }
    
    private void ShotSnowBall()
    {
        if(!animator.IsInTransition(0) &&!animator.GetCurrentAnimatorStateInfo(0).IsName("GatherStart") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Gathering") && !animator.GetCurrentAnimatorStateInfo(0).IsName("GatherFinish") && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpToTop") && !animator.GetCurrentAnimatorStateInfo(0).IsName("TopOfJump") && !animator.GetCurrentAnimatorStateInfo(0).IsName("TopToGround") /*&& !animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop")*/)
        {
            Bullet = transform.GetChild(3).gameObject;
            Vector3 ShotPos = Bullet.transform.position;
            Instantiate(ballPrefab, ShotPos, transform.rotation);
            Remaining--;
            animator.SetBool("Throw", true);
            Invoke(nameof(ThrowStop), 0.2f);
            YukidamaUI[Remaining].SetActive(false);
            PlayThrowSound(); // 音声を再生
        }
    }
    
    private void ShotRay()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.down);
        bool isGround = Physics.Raycast(ray, Distance);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            pos = hit.collider.gameObject.transform;   //rayの当たったオブジェクトの座標を取得
            block = hit.collider.gameObject;
            
            if (block.CompareTag("Grand") && Remaining < 10 && isGround && isGathering == false)
            {
                animator.SetBool("Gather", true);
                Invoke(nameof(Gather), 0.1f);
                isGathering = true;
            }
        }
    }
    private void Gather()
    {
        if (Remaining < 10 && block.CompareTag("Grand") && animator.GetCurrentAnimatorStateInfo(0).IsName("Gathering") && block.name != "Snow_0_3(Clone)")
        {
            Remaining++;
            YukidamaUI[Remaining - 1].SetActive(true);
            MapManager.instance.ChangeBlock(block, pos, DOWN);
            PlayGatheringSound(); // 音声の再生
        } else if(block.name == "Snow_0_3(Clone)"){
            animator.SetBool("Gather", false); // 雪が存在しないブロックの場合、Gatheringアニメーションを中止
        }
        isGathering = false;
    }
    private void ThrowStop()
    {
        animator.SetBool("Throw", false);
    }

    private void YukidamaDecrease()
    {
        if (Remaining > 0)
        {
            YukidamaUI[Remaining - 1].SetActive(false);
            Remaining--;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Takibi"))
        {
            ytime += Time.deltaTime;
            if (ytime >= YukidamaDeleteTime)
            {
                ytime = 0;
                YukidamaDecrease();

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Takibi"))
        {
            ytime = 0;
        }
    }

    // 音声関連
    private void PlayGatheringSound()
    {
        gatheringSound.pitch = 0.75f + Random.Range(-pitchRange, pitchRange);
        gatheringSound.PlayOneShot(gatheringSound.clip);
    }
    private void PlayThrowSound()
    {
        throwSound.pitch = 0.75f + Random.Range(-pitchRange, pitchRange);
        throwSound.PlayOneShot(throwSound.clip);
    }
}
