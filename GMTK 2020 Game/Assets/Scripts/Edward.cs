using UnityEngine;

public class Edward : MonoBehaviour
{
    [Header("Clipping")]
    public Transform[] endPoints;
    public Transform player;
    [Space]
    [Header("Spawning")]
    public GameObject[] objects;
    [Space]
    [Header("Voice Lines")]
    public AudioSource audioSource;
    public AudioClip[] voiceLines;

    public void Clip(int index)
    {
        player.transform.position = endPoints[index].position;
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
