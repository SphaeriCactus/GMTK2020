using UnityEngine;

public class BGMusic : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "End")
        {
            Destroy(gameObject);
        }
    }
}
