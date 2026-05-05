using UnityEngine;

public class Poop : MonoBehaviour
{
    public float fallSpeed = 3f;
    private float destroyY = -7f; // 화면 아래 경계

    void Update()
    {
        // 위에서 아래로 이동
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 화면 아래로 내려가면 삭제
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}