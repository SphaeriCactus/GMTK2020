using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingSequence : FakeGame
{
    private Edward ed;
    public TextMeshProUGUI controlText;

    public Color creditColor;

    public CanvasGroup menu;
    public GameObject game;

    public string[] credits;
    private int creditsCounter;

    // Start is called before the first frame update
    void Start()
    {
        ed = GameObject.FindWithTag("Edward").GetComponent<Edward>();
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        yield return new WaitForSeconds(4);
        ed.Speak(0);
        yield return new WaitForSeconds(6);

        //Remove "Control. \n"
        for(int i = 0; i < 9; i++)
        {
            controlText.text = controlText.text.Remove(0, 1);
            PlayBackspace();
            yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));
        }
        yield return new WaitForSeconds(1.5f);
        PlayBackspace();
        menu.alpha = 0;
        game.SetActive(true);

        StartCoroutine(DoControlPress());
    }

    protected override IEnumerator DoControlPress()
    {
        if(creditsCounter >= credits.Length)
        {
            End();
            yield break;
        }
        StartCoroutine(DisplayText(GetNextText(textColor), "PROCESSING INPUT..."));
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(DisplayText(GetNextText(textColor), "---===---"));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DisplayText(GetNextText(creditColor), credits[creditsCounter]));
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(DisplayText(GetNextText(textColor), "---====---"));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DisplayText(GetNextText(textColor), "DONE. NOW PRESS " + GetCommand()));

        creditsCounter++;
        yield return new WaitForSeconds(0.5f);
        PlayBackspace();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DoControlPress());
    }

    private void End()
    {
        Application.Quit();
    }

    void PlayBackspace()
    {
        ed.Effect(1, Random.Range(0.95f, 1.05f));
    }
}
