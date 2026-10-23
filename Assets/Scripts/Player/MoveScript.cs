using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class MoveScript : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] Camera _camera;
    [SerializeField] Rigidbody _rb;
    [SerializeField] GameObject _player;
    [SerializeField] Animator _animator;

    [Header("Config")]
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _rotationSpeed;

    private bool _canMove;
    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    private Vector3 _moveDirection;

    void Reset()
    {
        _speed = 10f;
        _jumpForce = 250f;
        _rotationSpeed = 200f;
    }

    void Start()
    {
        _canMove = true;
    }

    public void Move(Vector2 direction)
    {
        if (!_canMove)
            return;

        Vector3 directionFinale = new Vector3(direction.x, 0f, direction.y);

        Vector3 f = _camera.transform.forward;
        f.y = 0f;
        f.Normalize();

        Vector3 r = _camera.transform.right;
        r.y = 0f;
        r.Normalize();

        _moveDirection = (f * directionFinale.z) + (r * directionFinale.x);

        //_animator.SetBool("IsWalking", _moveDirection.sqrMagnitude > 0.01f);
    }

    void FixedUpdate()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 movement = _moveDirection.normalized * _speed * Time.fixedDeltaTime;

            _rb.MovePosition(_rb.position + movement);

            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            _rb.MoveRotation(
                Quaternion.Slerp(
                    _rb.rotation,
                    targetRotation,
                    _rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }
    public void Jump()
    {
        if (!_canMove)
            return;

        Vector3 down = transform.TransformDirection(Vector3.down);

        Debug.DrawRay(_rb.transform.position, down * 0.6f, Color.red);

        if (Physics.Raycast(_rb.transform.position, down, 0.6f))
        {
            //_animator.SetTrigger("Jumping");
            _rb.AddForce(new Vector3(0f, _jumpForce, 0f));
        }
    }
}