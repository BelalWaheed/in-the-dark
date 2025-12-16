using UnityEngine;

public class Background : MonoBehaviour
{
    private float startPos , length;
    public GameObject camera;
    public float parallaxEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = camera.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
        float movement = camera.transform.position.x * (1 - parallaxEffect);
        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
