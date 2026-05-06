using UnityEngine;

public class Poop : MonoBehaviour
{
    public float fallSpeed = 3f;
    private float destroyY = -7f; 

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}