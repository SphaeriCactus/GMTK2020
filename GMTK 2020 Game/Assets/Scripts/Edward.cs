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
    [Space]
    [Header("Misc")]
    public AudioSource effectSource;
    public AudioSource beep;
    public AudioClip[] effects;

    // For testing purposes
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            Clip(-10);
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

    public void Beep()
    {
        if(!beep.isPlaying)
            beep.Play();
    }

    public void Effect(int i, float pitch)
    {
        effectSource.clip = effects[i];
        effectSource.pitch = pitch;
        effectSource.Play();
    }

    public void Effect(int i)
    {
        Effect(i, 1);
    }
}
