using UnityEngine;

public class Poop : MonoBehaviour
{
    private float _fallSpeed;
    private const float DestroyY = -7f;

    void OnEnable()
    {
        _fallSpeed = Random.Range(2f, 12f);
    }

    void Update()
    {
        transform.position += Vector3.down * (_fallSpeed * Time.deltaTime);

        if (transform.position.y < DestroyY)
            ReturnToPool();
    }

    public void ReturnToPool()
    {
        PoopPool.Instance.Return(gameObject);
    }
}