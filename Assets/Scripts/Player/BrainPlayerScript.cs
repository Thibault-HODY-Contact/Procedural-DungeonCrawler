using UnityEngine;
using UnityEngine.InputSystem;

public class BrainPlayerScript : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;
    //[SerializeField] InputActionReference _interactInput;
    //[SerializeField] InputActionReference _pauseInput;
    [SerializeField] Animator _animator;

    [Header("Dependencies")]
    [SerializeField] MoveScript _moveScript;

    [SerializeField] LadderController _LadderScript;

    //[SerializeField] InteractionScript _interactionScript;
    [SerializeField] LifeScript _lifeScript;    
    //[SerializeField] PauseMenu _pauseMenu;
    void Update()
    {
        if (_lifeScript.Lives <= 0) return;

        _moveScript.Move(_moveInput.action.ReadValue<Vector2>());
        _LadderScript.Climb(_moveInput.action.ReadValue<Vector2>());
        _animator.SetFloat("Forward", _moveInput.action.ReadValue<Vector2>().y);
        _animator.SetFloat("Right", _moveInput.action.ReadValue<Vector2>().x);
        if (_jumpInput.action.WasPressedThisFrame()) _moveScript.Jump();

       // if (_interactInput.action.WasPressedThisFrame()) _interactionScript.Interact();

        //if (_pauseInput.action.WasPressedThisFrame())
        //{
        //    if (Time.timeScale == 0)
        //        _pauseMenu.Resume();
        //    else
        //        _pauseMenu.Pause();
        //}
    }
}