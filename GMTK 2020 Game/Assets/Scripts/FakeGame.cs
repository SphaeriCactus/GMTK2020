using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FakeGame : MonoBehaviour
{
    public Queue<TextMeshProUGUI> textQ;
    public List<TextMeshProUGUI> textList;

    public Color textColor;

    public static bool isAwaitingInput;
    private int i;

    private int counter;
    private KeyCode keyToPress;
    private KeyCode secondKey;

    private void Awake()
    {
        isAwaitingInput = false;
        textQ = new Queue<TextMeshProUGUI>();
        textQ.Enqueue(textList[0]);

        keyToPress = KeyCode.RightControl;
        secondKey = KeyCode.LeftControl;
    }

    private void LateUpdate()
    {
        //Lol sorry mac users
        if(isAwaitingInput && (Input.GetKeyDown(keyToPress) || Input.GetKeyDown(secondKey)))
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
        StartCoroutine(DisplayText(GetNextText(), "DONE. NOW PRESS " + GetCommand()));
        isAwaitingInput = true;
    }

    private string GetCommand()
    {
        counter++;
        if (counter > 2)
            counter = 0;
        switch(counter)
        {
            default:
                if(SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
                {
                    keyToPress = KeyCode.LeftCommand;
                    secondKey = KeyCode.RightCommand;
                    return "CMD.";
                }
                else
                {
                    keyToPress = KeyCode.LeftControl;
                    secondKey = KeyCode.RightControl;
                    return "CTRL.";
                }
            case 1:
                keyToPress = KeyCode.LeftAlt;
                secondKey = KeyCode.RightAlt;
                return "ALT.";
            case 2:
                keyToPress = KeyCode.Delete;
                secondKey = KeyCode.Delete;
                return "DELETE.";
        }
    }

    private TextMeshProUGUI GetNextText()
    {
        if(i == textList.Capacity-1) //Reuse text
        {
            TextMeshProUGUI t = textQ.Dequeue();
            Transform p = t.transform.parent;
            t.transform.SetParent(null);
            t.transform.SetParent(p);
            textQ.Enqueue(t);
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
