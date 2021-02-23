using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// handles displaying helper messages for player (f.e item "out of range" for pickup)
/// </summary>
public class HelperText : MonoBehaviour
{
    TextMeshProUGUI textControl;
    Fader fader;
    static HelperText instance;

    private void Awake()
    {
        textControl = GetComponent<TextMeshProUGUI>();
        fader = GetComponent<Fader>();
        if (instance == null)
            instance = this;
    }

    public static void DisplayText(string msg, float duration)
    {
        if (instance != null)
            instance.Display(msg, duration);
    }

    public void Display(string msg, float duration)
    {
        StartCoroutine(DisplayMessage(msg, duration));
    }

    IEnumerator DisplayMessage(string msg, float duration)
    {
        textControl.text = msg;
        fader.FadeIn();
        yield return new WaitForSeconds(duration);
        fader.FadeOut();
    }
}
