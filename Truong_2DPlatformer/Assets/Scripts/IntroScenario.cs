using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScenario : MonoBehaviour
{
    [SerializeField] Parrot parrot;
    [SerializeField] Character player;
    [SerializeField] Transform parrotTarget1;
    static bool playIntro;
    
    void Start()
    {
        Debug.Log("Intro Start");
        parrot.gameObject.SetActive(playIntro);
    }
    
    void Update()
    {
        
    }

    public void Play()
    {
        StartCoroutine(PlayScenario());
    }

    IEnumerator PlayScenario()
    {
        playIntro = true;
        Debug.Log("Intro Play");
        parrot.gameObject.SetActive(true);
        player.Freeze();
        player.LayDown();
        yield return new WaitForSeconds(2);
        parrot.Eat();
        parrot.PlayRandomSound();
        yield return new WaitForSeconds(2);
        parrot.Eat();
        parrot.PlayRandomSound();
        yield return new WaitForSeconds(1);
        player.StandUp();
        yield return new WaitForSeconds(2);
        player.Unfreeze();
        parrot.FlyTo(parrotTarget1.transform.position);

    }

    public void Skip()
    {
        player.Idle();
    }
}
