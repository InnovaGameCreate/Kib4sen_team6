using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    private Vector3 pos; //�u���b�N��ݒu����ʒu
    [SerializeField]
    private GameObject blockPrefab;
    private Rigidbody ballRigid;
    [SerializeField]
    private float force = 1f;
    // Start is called before the first frame update
    void Start()
    {
        ballRigid = this.GetComponent<Rigidbody>();
        ballRigid.AddForce(new Vector3(force, 0, 0));

    }

    // Update is called once per frame
    void Update()
    {
        //�{�[���̈ʒu���瑬�x�����֔�΂�
        Ray ray = new Ray(this.transform.position,ballRigid.velocity);
        //���������I�u�W�F�N�g�����i�[����ϐ�
        RaycastHit hit;

        //Physics.Raycast() �Ń��C���΂�
        if (Physics.Raycast(ray, out hit))
        {
            //�Փ˂����ʂ̕���+�u���b�N�̍��W
            pos = hit.normal / 2 + hit.collider.transform.position;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
        Instantiate(blockPrefab, pos, Quaternion.identity);

    }
}
