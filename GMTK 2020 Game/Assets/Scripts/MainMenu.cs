using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup group;
    public GameObject game;
    public Edward edward;

    private bool menuEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (menuEnabled)
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
                edward.Speak(0);
            }
        }
    }
}
