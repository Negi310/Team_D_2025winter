using UnityEngine;

public class SalmonMove : MonoBehaviour
{
    public SalmonData salmonData;

    float speed;
    //public float speed = 5f;
    Vector2 move;

    void Start()
    {
        speed = salmonData.UpstreamStats.Speed * 0.5f;
    }
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
