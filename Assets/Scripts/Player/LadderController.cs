using System;
using Unity.VisualScripting;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    [SerializeField] MoveScript moveScript;

    [SerializeField] Animator animator;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _speed;
    [SerializeField] GameObject player;
    private RigidbodyConstraints _baseConstraints;

    private Vector3 _moveDirection;


    public void Climb(Vector2 direction)
    {
        if (animator.GetBool("isClimbing"))
        {
            _moveDirection = new Vector3(0, direction.y,0);
        }

    }
    void FixedUpdate()
    {


        if (animator.GetBool("isClimbing"))
        {


            _rb.linearVelocity = new Vector3(0, _moveDirection.y * _speed, 0);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        var layerMask = other.gameObject.layer;


        if (layerMask == LayerMask.NameToLayer("ClimbLadder"))
        {
            moveScript.CanMove = false;
            animator.SetBool("isClimbing", true);

            Vector3 ladderCenter = other.bounds.center;
            Vector3 ladderAtPlayerHeight = new Vector3(ladderCenter.x, _rb.position.y, ladderCenter.z);

            Vector3 toPlayer = _rb.position - ladderAtPlayerHeight;
            toPlayer.y = 0f;
            toPlayer.Normalize();

            _rb.position = ladderAtPlayerHeight + toPlayer * 0.25f;   
            _rb.rotation = Quaternion.LookRotation(-toPlayer);       
            _rb.linearVelocity = Vector3.zero;

            _rb.useGravity = false;
            _baseConstraints = _rb.constraints;
            _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else if (layerMask == LayerMask.NameToLayer("UnClimbLadder") && animator.GetBool("isClimbing"))
        {
            animator.SetBool("isClimbing", false);
            _rb.linearVelocity = Vector3.zero;
            _rb.isKinematic = true;

        }
        if (layerMask == LayerMask.NameToLayer("OnGround") && !animator.GetBool("isClimbing"))
        {
            moveScript.CanMove = true;
            _rb.useGravity = true;
            _rb.constraints = _baseConstraints;
            _rb.isKinematic = false;
        }
    }
}
