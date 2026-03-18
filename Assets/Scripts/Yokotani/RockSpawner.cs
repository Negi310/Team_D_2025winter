using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rock;
    void Start()
    {
        InvokeRepeating("Spawn", 1f, 3f);
    }

    void Spawn()
    {
        float x = Random.Range(-4f, 4f);
        Instantiate(rock, new Vector3(x, 6, 0), Quaternion.identity);
    }
}
