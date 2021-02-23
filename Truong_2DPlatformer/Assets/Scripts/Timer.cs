using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// component that invokes event after specified time
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField] float duration;

    public UnityEvent expired;

    public void StartTimer()
    {
        StartCoroutine(InvokeEventWithDelay());
    }
    
    IEnumerator InvokeEventWithDelay()
    {
        yield return new WaitForSeconds(duration);
        expired?.Invoke();
    }
}
