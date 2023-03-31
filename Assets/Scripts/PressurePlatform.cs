using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatform : MonoBehaviour
{
    public float downSpeed = 1f; // 下降速度
    public float upSpeed = 1f; // 上升速度
    public float maxDownDistance = 1f; // 最大下降距离
    public bool isPlayerOnPlate = false; // 是否有玩家在平台上
    public float reboundDelay = 1f; // 回弹延迟时间

    private float initY; // 初始y轴坐标

    void Start()
    {
        initY = transform.position.y;
    }

    void Update()
    {
        if (isPlayerOnPlate)
        {
            // 计算下降距离
            float distance = Mathf.Clamp(initY - transform.position.y, 0, maxDownDistance);

            // 计算下降速度
            float speed = downSpeed * (1 - distance / maxDownDistance);

            // 下降
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        else
        {
            // 计算上升速度
            float speed = upSpeed;

            // 上升
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            // 到达初始高度后停止上升
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
