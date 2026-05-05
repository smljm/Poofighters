using UnityEngine;

public class GroundAnchor : MonoBehaviour
{
    public float heightFromBottom = 0.5f; // 바닥에서 얼마나 위에

    void Start()
    {
        Camera cam = Camera.main;
        float bottomY = cam.transform.position.y - cam.orthographicSize;
        float groundY = bottomY + heightFromBottom;
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);

        // X 스케일도 화면 너비에 맞춤
        float screenWidth = cam.orthographicSize * 2f * cam.aspect;
        transform.localScale = new Vector3(screenWidth, 1f, 1f);
    }
}