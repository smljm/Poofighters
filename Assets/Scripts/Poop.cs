using UnityEngine;

public class Poop : MonoBehaviour
{
    float fallSpeed;
    float destroyY = -7f;

    void Start()
    {
        fallSpeed = Random.Range(2f, 12f);
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < destroyY)
            Destroy(gameObject);
    }
}