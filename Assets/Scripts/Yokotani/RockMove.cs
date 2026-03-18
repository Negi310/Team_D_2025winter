using UnityEngine;

public class RockMove : MonoBehaviour
{
    public float speed = 3f;
    void Update()
    {
        transform.Translate(0, -speed * Time.deltaTime, 0);

        if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }
}
