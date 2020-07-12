using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnConveyor : MonoBehaviour
{
    public Conveyor conveyor;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(TurnOn());
    }

    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(12.4f);
        conveyor.shouldBeOn = true;
    }
}
