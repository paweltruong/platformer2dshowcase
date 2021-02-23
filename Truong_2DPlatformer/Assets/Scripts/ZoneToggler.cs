using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// allow to fire events when player enters or exits trigger zone
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZoneToggler : MonoBehaviour
{
    public UnityEvent onEnterLeft;
    public UnityEvent onEnterRight;
    public UnityEvent onEnter;

    public UnityEvent onExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO:refactor to more accurate
        var enteringRigidBody = collision.GetComponent<Rigidbody2D>();
        if(enteringRigidBody != null && enteringRigidBody.tag == Constants.PlayerTag)
        {
            if (enteringRigidBody.velocity.x > 0)
                onEnterLeft?.Invoke();
            else
                onEnterRight?.Invoke();
            onEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var exitingRigidBody = collision.GetComponent<Rigidbody2D>();
        if (exitingRigidBody != null && exitingRigidBody.tag == Constants.PlayerTag)
        {
            onExit?.Invoke();
        }
    }
}
