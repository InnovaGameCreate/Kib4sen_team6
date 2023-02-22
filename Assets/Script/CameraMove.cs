using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float x, y, z;
    float speed = 0.1f;

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;

    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -60f, maxX = 47f;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        //Updateの中で作成した関数を呼ぶ
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

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        //transform.position += new Vector3(x,y,z);

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("TopOfJump") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running@loop"))
        {
            transform.position += cam.transform.forward * z * 2 / 5 + cam.transform.right * x * 2 / 5 + cam.transform.forward * y * 0 + cam.transform.right * y * 0;
        }

        if (x > 0 || x < 0 || z > 0 || z < 0)
        {
            //animator.SetBool("Running", true);
        }
        else
        {
            //animator.SetBool("Running", false);
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

    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }


}
