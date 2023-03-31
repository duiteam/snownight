using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatform : MonoBehaviour
{
    public float downSpeed = 1f; // �½��ٶ�
    public float upSpeed = 1f; // �����ٶ�
    public float maxDownDistance = 1f; // ����½�����
    public bool isPlayerOnPlate = false; // �Ƿ��������ƽ̨��
    public float reboundDelay = 1f; // �ص��ӳ�ʱ��

    private float initY; // ��ʼy������

    void Start()
    {
        initY = transform.position.y;
    }

    void Update()
    {
        if (isPlayerOnPlate)
        {
            // �����½�����
            float distance = Mathf.Clamp(initY - transform.position.y, 0, maxDownDistance);

            // �����½��ٶ�
            float speed = downSpeed * (1 - distance / maxDownDistance);

            // �½�
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        else
        {
            // ���������ٶ�
            float speed = upSpeed;

            // ����
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            // �����ʼ�߶Ⱥ�ֹͣ����
            if (transform.position.y >= initY)
            {
                transform.position = new Vector3(transform.position.x, initY, transform.position.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnPlate = true;

            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ReboundAfterDelay());
            collision.transform.SetParent(null);
        }
    }

    public IEnumerator ReboundAfterDelay()
    {
        yield return new WaitForSeconds(reboundDelay);
        isPlayerOnPlate = false;
    }
}
