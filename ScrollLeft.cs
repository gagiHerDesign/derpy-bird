using UnityEngine;

/// <summary>Moves the object left and destroys it once off-screen (works for obstacles and backgrounds).</summary>
public class ScrollLeft : MonoBehaviour
{
    public float speed = 2f;
    public float destroyX = -15f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < destroyX) Destroy(gameObject);
    }
}
