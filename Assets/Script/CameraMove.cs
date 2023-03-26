using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CameraMove : MonoBehaviour
{
    float x, y, z;
    [SerializeField] private float runningSpeed = 0.1f;
    [SerializeField] private float gatheringSpeed = 0.1f;

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    public float Xsensityvity = 3f, Ysensityvity = 3f;

    bool cursorLock = true;

    //�ϐ��̐錾(�p�x�̐����p)
    float minX = -60f, maxX = 47f;

    // 音声関連
    private AudioSource runningSound; // 走るSE
    [SerializeField] float pitchRange = 0.1f; // ピッチのランダム幅
    //

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
        animator = GetComponent<Animator>();
        // 音声の取得
        runningSound = GetComponents<AudioSource>()[4];
        //
    }

    // Update is called once per frame
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        //Update�̒��ō쐬�����֐����Ă�
        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;


        UpdateCursorLock();
    }

    private void FixedUpdate()
    {
        x = 0;
        y = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        //transform.position += new Vector3(x,y,z);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TopOfJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop") || animator.GetCurrentAnimatorStateInfo(0).IsName("JumpToTop"))
        {
            transform.position += (cam.transform.forward * z + cam.transform.right * x).normalized * 2 / 5 * runningSpeed;
        }

        if (x > 0 || x < 0 || z > 0 || z < 0)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }

        // Gathering中の移動
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Gathering"))
        {
            transform.position += (cam.transform.forward * z + cam.transform.right * x).normalized * 2 / 5 * gatheringSpeed;
        }
    }


    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }


        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //�p�x�����֐��̍쐬
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,z�̓x�N�g���i�ʂƌ����j�Fw�̓X�J���[�i���W�Ƃ͖��֌W�̗ʁj)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    // 走る音声を再生する関数
    private void FootStepSE()
    {
        runningSound.pitch = 0.75f + Random.Range(-pitchRange, pitchRange);
        runningSound.PlayOneShot(runningSound.clip);
    }
}
