using System.Collections;
using UnityEngine;

public class DevTrigger : MonoBehaviour
{
    private Edward edward;
    public enum DevAction {Clip, Spawn, Speak, Enable};
    public DevAction toDo;
    public int index;

    public float waitTime = 0;

    public bool canTrigger;
    public DevTrigger toDisable;
    public MonoBehaviour scriptToEnable;


    void Start()
    {
        edward = GameObject.FindWithTag("Edward").GetComponent<Edward>();
        canTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canTrigger)
        {
            StartCoroutine(DoAction());
            canTrigger = false;
            if (toDisable != null)
                toDisable.canTrigger = false;
        }
    }

    IEnumerator DoAction()
    {
        yield return new WaitForSeconds(waitTime);
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
            case DevAction.Enable:
                edward.Enable(scriptToEnable);
                break;
        }
    }
}
