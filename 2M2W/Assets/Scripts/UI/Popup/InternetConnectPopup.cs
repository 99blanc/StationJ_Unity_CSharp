using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InternetConnectPopup : UIPopup
{
    private enum Buttons
    {
        Button
    }

    public override void Init()
    {
        base.Init();

        BindButton(typeof(Buttons));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void ProcessButton(Buttons button)
    {
        switch (button)
        {
            case Buttons.Button:
                Application.Quit();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }
}

