using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FakeGame : MonoBehaviour
{
    public Queue<TextMeshProUGUI> textQ;
    public List<TextMeshProUGUI> textList;

    public Color textColor;

    private bool isAwaitingInput;
    private int i;

    private void Awake()
    {
        isAwaitingInput = true;
        textQ = new Queue<TextMeshProUGUI>();
        textQ.Enqueue(textList[0]);
    }

    private void Update()
    {
        //Lol sorry mac users
        if(isAwaitingInput && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)))
        {
            Debug.Log("Pressed");
            StartCoroutine(DoControlPress());
        }
    }

    private IEnumerator DoControlPress()
    {
        isAwaitingInput = false;
        StartCoroutine(DisplayText(GetNextText(), "PROCESSING INPUT..."));
        yield return new WaitForSeconds(1.2f);
        TextMeshProUGUI next = GetNextText();
        next.text = "";
        for (int t = 0; t < 3; t++)
        {
            next.text += ".";
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(DisplayText(GetNextText(), "DONE. PRESS CTRL ONCE MORE."));
        isAwaitingInput = true;
    }

    private TextMeshProUGUI GetNextText()
    {
        if(i == textList.Capacity-1) //Reuse text
        {
            TextMeshProUGUI t = textQ.Dequeue();
            Transform p = t.transform.parent;
            t.transform.SetParent(null);
            t.transform.SetParent(p);
            return t;
        }
        else
        {
            i++;
            textList[i].color = textColor;
            textQ.Enqueue(textList[i]);
            return textList[i];
        }
    }
    
    private IEnumerator DisplayText(TextMeshProUGUI text, string s)
    {
        text.text = "";
        foreach (char c in s)
        {
            text.text += c;
            yield return new WaitForEndOfFrame();
        }
    }
}
