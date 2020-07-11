using UnityEngine;

public class Edward : MonoBehaviour
{
    [Header("Clipping")]
    public Transform[] endPoints;
    public Transform player;
    public Transform respawnPoint;
    [Space]
    [Header("Spawning")]
    public GameObject[] objects;
    [Space]
    [Header("Voice Lines")]
    public AudioSource audioSource;
    public AudioClip[] voiceLines;

    // For testing purposes
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            Clip(0);
        }
    }

    public void Clip(int index)
    {
        if (index == -10)
        {
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;
        } else 
        {
            player.transform.position = endPoints[index].position;
            player.transform.rotation = endPoints[index].rotation;
        }
    }

    public void Spawn(int index)
    {
        objects[index].SetActive(true);
    }

    public void Speak(int index)
    {
        audioSource.clip = voiceLines[index];
        audioSource.Play();
    }
}
