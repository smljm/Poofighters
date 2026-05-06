using UnityEngine;

public class PoopSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject poopPrefab;      
    public float spawnInterval = 1f;   
    public float minX = -8f;           
    public float maxX = 8f;             
    public float spawnY = 7f;         

    [Header("난이도 증가")]
    public float intervalDecreaseRate = 0.05f;
    public float minInterval = 0.2f;           

    private float timer = 0f;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPoop();
            timer = 0f;
            
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