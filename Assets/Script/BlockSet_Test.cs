using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSet_Test : MonoBehaviour
{
    Vector2 displayCenter;

    // �u���b�N��ݒu����ʒu���ꉞ���A���^�C���Ŋi�[
    private Vector3 pos;

    [SerializeField]
    private GameObject blockPrefab;

    // Use this for initialization
    void Start()
    {
        //��ʒ����̕��ʍ��W���擾����
        displayCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        //�u�J��������̃��C�v����ʒ����̕��ʍ��W�����΂�
        Ray ray = Camera.main.ScreenPointToRay(displayCenter);
        //���������I�u�W�F�N�g�����i�[����ϐ�
        RaycastHit hit;

        //Physics.Raycast() �Ń��C���΂�
        if (Physics.Raycast(ray, out hit))
        {
            //�Փ˂����ʂ̕���+�u���b�N�̍��W
            pos = hit.normal/2 + hit.collider.transform.position;

            //�E�N���b�N
            if (Input.GetMouseButtonDown(1))
            {
                //�����ʒu�̕ϐ��̍��W�Ƀu���b�N�𐶐�
                Instantiate(blockPrefab, pos, Quaternion.identity);
            }

            //���N���b�N
            if (Input.GetMouseButtonDown(0))
            {
                if(hit.collider.gameObject.tag == "Snow")
                Destroy(hit.collider.gameObject);   //�Ώ̃u���b�N��j��
            }
        }
    }
}
