using UnityEngine;

/// <summary>Spawns an obstacle on the right side at a fixed interval, at a random height.</summary>
public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;     // prefab: top and bottom pipes with a gap between them
    public float interval = 2.5f;     // voice control is slower than tapping, so keep spacing generous
    public float minY = -1.5f;
    public float maxY = 1.5f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < interval) return;
        timer = 0f;
        Vector3 pos = transform.position + Vector3.up * Random.Range(minY, maxY);
        Instantiate(pipePrefab, pos, Quaternion.identity);
    }
}
