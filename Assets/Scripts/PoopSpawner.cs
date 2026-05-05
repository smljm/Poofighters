using UnityEngine;

public class PoopSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject poopPrefab;       // 인스펙터에서 프리팹 연결
    public float spawnInterval = 1f;    // 스폰 간격 (초)
    public float minX = -8f;            // 스폰 X 최솟값
    public float maxX = 8f;             // 스폰 X 최댓값
    public float spawnY = 7f;           // 스폰 Y 위치 (화면 위)

    [Header("난이도 증가")]
    public float intervalDecreaseRate = 0.05f; // 매 스폰마다 간격 감소량
    public float minInterval = 0.2f;           // 최소 스폰 간격

    private float timer = 0f;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPoop();
            timer = 0f;

            // 난이도 증가: 점점 빠르게 스폰
            spawnInterval = Mathf.Max(minInterval, spawnInterval - intervalDecreaseRate);
        }
    }

    void SpawnPoop()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);
        Instantiate(poopPrefab, spawnPos, Quaternion.identity);
    }
}