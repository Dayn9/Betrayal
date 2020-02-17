using UnityEngine;

public class QuitButton : UIButton
{
    protected override void OnActive() { }

    protected override void OnClick()
    {
        Application.Quit();
    }

    protected override void OnEnter() { }
}

