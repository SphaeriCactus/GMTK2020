using UnityEngine;

public class Door : MonoBehaviour
{
    private Edward edward;
    void Start()
    {
        edward = GameObject.FindWithTag("Edward").GetComponent<Edward>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            edward.Clip(0);
        }
    }
}
