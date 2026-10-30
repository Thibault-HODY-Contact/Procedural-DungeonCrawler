using System;
using UnityEngine;

public class LadderAnimFunc : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    public void OnGround()
    {
        _rb.position = _rb.position + animator.deltaPosition + _rb.transform.forward + _rb.transform.up;
    }

}
