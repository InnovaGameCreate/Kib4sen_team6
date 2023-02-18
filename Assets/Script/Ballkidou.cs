using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballkidou : MonoBehaviour
{
    //��ʂɂ�������X�N���v�g
    private GameObject snowball;
    [SerializeField] private float speed;
    private Vector3 pos;
    private float tyusinkyori;
    [SerializeField] private float okuyuki; //��ʒ��S�̉��s���B�J������x�̊p�x�̒l��0�ȉ��̎��ǂꂭ�炢�̉��s���Ő�ʂ���ʒ��S��ʂ邩�̒l
    [SerializeField] private float sitanageRot; //�J������x�̊p�x�����̒l�ȏ�ɂȂ�����n�ʂɉ����悤�ɐ�ʂ𓊂��Ȃ��Ȃ�
    private float x;
    private Vector3 uiballpos;
    private Ray kidou;
    private Vector3 accel;
    [SerializeField] private float waittime;
    private Vector3 force;
    [SerializeField] private float gensoku; //�ŏI�I�ɂǂ�ʌ������邩
    [SerializeField] private float gravity; //�d�͂̐��l
    [SerializeField] private float gkasokukankaku; //�ǂꂭ�炢�̎��Ԃ̊Ԋu�Ō������邩
    [SerializeField] [Tooltip("BreakSnowEffect")] private ParticleSystem particle;
    private ParticleSystem effinst;
    const int UP = 1;   //����オ�鎞�̃t���O(SnowBall�̂��̂Ɠ���)

    // Start is called before the first frame update
    void Start()   //��ʂ���ʒ��S�ɔ��ł������߂̑O����
    {
        Quaternion qua = Camera.main.transform.rotation;
        x = qua.eulerAngles.x * 3.14f / 180;
        if (Camera.main.transform.rotation.x > 0)
        {
            tyusinkyori = Camera.main.transform.position.y / Mathf.Sin(x);
        }
        else
        {
            tyusinkyori = okuyuki;
        }
        uiballpos = new Vector3(Screen.width / 2, Screen.height / 2, tyusinkyori);  //��ʒ��S�̍��W
        pos = Camera.main.ScreenToWorldPoint(uiballpos);
        kidou = new Ray(transform.position, pos - transform.position);   //��ʂ̔��ˈʒu�����ʒ��S�����ɔ��ł���ray

        force = kidou.direction * speed;
        if (0 < Camera.main.transform.rotation.x && Camera.main.transform.rotation.x < sitanageRot*Mathf.PI / 180f)
        {
            force.y = 0;
            Debug.Log("a");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.GetComponent<Rigidbody>().AddForce(force); //��ʂ��^���������ł���

        StartCoroutine(wait(kidou));
    }

    private IEnumerator wait(Ray kidou)
    {
        yield return new WaitForSeconds(waittime); //waittime�b��ɒ����^������߂ė������n�߂�

        for (int i = 0; i < gensoku; i++)  //���X�Ɍ���
        {
            yield return new WaitForSeconds(0.1f);
            accel = new Vector3(kidou.direction.x * speed * -1 / gensoku, 0, kidou.direction.z * speed * -1 / gensoku); 
            this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
        }

        while (this.transform.position.y > 0)  //��ʂ�y���W��0�ɂȂ�܂�y�������̕��̕����ɉ���
        {
            yield return new WaitForSeconds(gkasokukankaku);
            fall(kidou);
        }
    }

    void fall(Ray kidou)
    {
        accel = new Vector3(0, -gravity, 0);
        this.gameObject.GetComponent<Rigidbody>().AddForce(accel);
    }

    private void OnTriggerEnter(Collider other)
    {
        effinst = Instantiate(particle);
        effinst.transform.position = this.transform.position;
        effinst.Play();
        Destroy(effinst.gameObject, 3.0f);
        if (other.gameObject.tag == "Grand") //SnowBall�Ɠ���̂���
        {
            MapManager.instance.ChangeBlock(other.gameObject, other.gameObject.transform, UP);  //�u���b�N�̐ݒu
        }
        Destroy(this.gameObject);
    }
}
