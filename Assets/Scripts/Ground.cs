using UnityEngine;

public class GroundAnchor : MonoBehaviour
{
    public float heightFromBottom = 0.5f;

    void Start()
    {
        Camera cam = Camera.main;
        float bottomY = cam.transform.position.y - cam.orthographicSize;
        float groundY = bottomY + heightFromBottom;
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        
        float screenWidth = cam.orthographicSize * 2f * cam.aspect;
        transform.localScale = new Vector3(screenWidth, 1f, 1f);
    }
}