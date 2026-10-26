using System;
using UnityEngine;

public class LadderAnimFunc : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    public void OnGround()
    {
        _rb.position = _rb.position + animator.deltaPosition + _rb.transform.forward + _rb.transform.up;
        Debug.Log("aap");
    }
        
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
