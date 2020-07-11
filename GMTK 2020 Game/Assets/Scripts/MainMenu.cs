using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup group;
    public CanvasGroup buttons;
    public GameObject game;
    public Edward edward;

    private bool canPress = false;

    private bool menuEnabled = true;

    IEnumerator Start()
    {
        edward.Speak(0);
        yield return new WaitForSeconds(15);
        canPress = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canPress == true)
        {
            buttons.alpha = 1;
        }

        if (menuEnabled && canPress)
        {
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                group.alpha = 0;
                game.SetActive(true);
                FakeGame.isAwaitingInput = true;
                menuEnabled = false;
            }
        }
    }
}
