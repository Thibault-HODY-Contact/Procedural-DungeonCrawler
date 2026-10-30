using UnityEngine;

public class LadderController : MonoBehaviour
{
    [SerializeField] MoveScript moveScript;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _speed;
    [SerializeField] float _offsetTowardLadder = 0.2f;
    [SerializeField] GameObject player;

    private RigidbodyConstraints _baseConstraints;
    private float _climbInput;

    bool IsClimbing => animator.GetBool("isClimbing");

    void Awake()
    {
        _baseConstraints = _rb.constraints;
    }

    public void Climb(Vector2 direction)
    {
        _climbInput = direction.y;
    }

    public void UnClimb()
    {
        if (IsClimbing)
        {
            animator.SetBool("isClimbing", false);
            RestoreWalking();
        }
    }

    void FixedUpdate()
    {
        if (IsClimbing)
            _rb.linearVelocity = new Vector3(0f, _climbInput * _speed, 0f);
    }

    private void OnTriggerStay(Collider other)
    {
        int layer = other.gameObject.layer;

        if (layer == LayerMask.NameToLayer("ClimbLadder") && !IsClimbing && _climbInput > 0f)
            StartClimbing(other);
        else if (layer == LayerMask.NameToLayer("UnClimbLadder") && IsClimbing)
            ReachTop();
        else if (layer == LayerMask.NameToLayer("OnGround") && !IsClimbing && _rb.isKinematic)
            RestoreWalking();
    }

    void StartClimbing(Collider ladder)
    {
        RestartGas();

        moveScript.CanMove = false;
        animator.SetBool("isClimbing", true);

        Vector3 hatchCenter = ladder.transform.parent.position;
        Vector3 toLadder = ladder.bounds.center - hatchCenter;
        toLadder.y = 0f;
        if (toLadder.sqrMagnitude < 0.0001f) toLadder = _rb.transform.forward;
        toLadder.Normalize();

        Vector3 climbPoint = hatchCenter + toLadder * _offsetTowardLadder;

        _rb.isKinematic = false;
        _rb.useGravity = false;
        _rb.position = new Vector3(climbPoint.x, _rb.position.y, climbPoint.z);
        _rb.rotation = Quaternion.LookRotation(toLadder);
        _rb.linearVelocity = Vector3.zero;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void ReachTop()
    {
        animator.SetBool("isClimbing", false);
        _rb.linearVelocity = Vector3.zero;
        _rb.isKinematic = true;
    }

    void RestoreWalking()
    {
        moveScript.CanMove = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.constraints = _baseConstraints;
    }

    void RestartGas()
    {
        if (GameManager.Instance == null || GameManager.Instance.genGenerator == null) return;

        GenGenerator generator = GameManager.Instance.genGenerator.GetComponent<GenGenerator>();
        if (generator != null) generator.RestartGasAtPlayer();
    }
}