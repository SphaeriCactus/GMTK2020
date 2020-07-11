using UnityEngine;

public class DevTrigger : MonoBehaviour
{
    private Edward edward;
    public enum DevAction {Clip, Spawn, Speak};
    public DevAction toDo;
    public int index;

    void Start()
    {
        edward = GameObject.FindWithTag("Edward").GetComponent<Edward>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DoAction();
            Destroy(gameObject);
        }
    }

    void DoAction() {
        switch (toDo)
        {
            case DevAction.Clip:
                edward.Clip(index);
                break;
            case DevAction.Spawn:
                edward.Spawn(index);
                break;
            case DevAction.Speak:
                edward.Speak(index);
                break;
        }
    }
}
