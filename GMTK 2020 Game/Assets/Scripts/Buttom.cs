using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttom : MonoBehaviour
{
    private bool inside = false;
    public Edward edward;
    public CameraLook cameraMovement;
    public PlayerController playerMovement;
    private GameObject music;

    void Start()
    {
        music = GameObject.FindWithTag("Music");
    }
    
    void Update()
    {
        if (inside && Input.GetKeyDown("e"))
        {
            StartCoroutine("Press");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inside = false;
        }
    }

    IEnumerator Press()
    {
        edward.Speak(5);
        yield return new WaitForSeconds(18.1f);
        cameraMovement.enabled = false;
        playerMovement.enabled = false;
        yield return new WaitForSeconds(6.5f);
        Destroy(music);
        SceneManager.LoadScene("End");
    }
}
