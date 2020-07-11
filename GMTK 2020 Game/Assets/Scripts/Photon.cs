using UnityEngine;
using UnityEngine.SceneManagement;

public class Photon : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("MinusAHalfRealm");
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 500);
        }
    }
}
