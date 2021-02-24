using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFadeWithDistance : MonoBehaviour
{
    [SerializeField] Transform listenerTransform;
    AudioSource audioSource;
    public float minDist = 1;
    public float maxDist = 400;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, listenerTransform.position);

        if (distance < minDist)
            audioSource.volume = 1;
        else if (distance > maxDist)
            audioSource.volume = 0;
        else
            audioSource.volume = 1 - ((distance - minDist) / (maxDist - minDist));
    }

    void OnDrawGizmos()
    {
        var leftMin = new Vector3(transform.position.x - minDist, transform.position.y, transform.position.z);
        var leftRange = new Vector3(transform.position.x - maxDist, transform.position.y, transform.position.z);
        var rightMin = new Vector3(transform.position.x + minDist, transform.position.y, transform.position.z);
        var rightRange = new Vector3(transform.position.x + maxDist, transform.position.y, transform.position.z);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftMin, leftRange);
        Gizmos.DrawLine(rightMin, rightRange);
    }
}
