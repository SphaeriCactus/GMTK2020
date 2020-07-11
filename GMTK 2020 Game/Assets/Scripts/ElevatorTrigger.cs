using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("open");
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
