using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrot : MonoBehaviour
{
    [SerializeField] float flySpeed;
    [SerializeField] AudioClip[] sounds;
    [SerializeField] AudioClip[] wingSounds;

    float destinationMargin = 1f;
    Animator anim;
    AudioSource audioSource;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Eat()
    {
        anim.SetTrigger(Constants.AnimationParameters.Trigger_Eat);
    }

    public void FlyTo(Vector3 position)
    {
        StartCoroutine(FlyToPoint(position));
    }

    IEnumerator FlyToPoint(Vector3 position)
    {
        anim.SetBool(Constants.AnimationParameters.Trigger_Fly, true);
        while (Vector2.Distance((Vector2)transform.position, (Vector2)position) > destinationMargin)
        {
            //update parrot sprite flip (direction)
            if ((position - transform.position).normalized.x >= 0)
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);

            yield return new WaitForFixedUpdate();
            transform.position = Vector3.MoveTowards(transform.position, position, flySpeed * Time.fixedDeltaTime);
        }
    }

    public void PlayRandomSound()
    {
        Utils.PlayRandomSound(audioSource, sounds);
    }

    public void PlayRandomWingFlap()
    {
        Utils.PlayRandomSound(audioSource, wingSounds);
    }
}
