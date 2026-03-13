using UnityEngine;

public class SalmonMove : MonoBehaviour
{
    public float speed = 5f;
    Vector2 move;

    void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        transform.Translate(move * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rock"))
        {
            Debug.Log("Hit");
        }
    }
}
