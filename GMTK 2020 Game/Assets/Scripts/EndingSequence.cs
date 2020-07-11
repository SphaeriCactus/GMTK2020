using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingSequence : MonoBehaviour
{

    public TextMeshProUGUI controlText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        yield return new WaitForSeconds(2);

        //Remove "Control. \n"
        for(int i = 0; i < 9; i++)
        {
            controlText.text = controlText.text.Remove(0, 1);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
