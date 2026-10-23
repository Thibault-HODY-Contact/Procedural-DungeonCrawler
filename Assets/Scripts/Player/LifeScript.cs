using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LifeScript : MonoBehaviour, IDamageable
{
    [Header("Dependencies")]
    [SerializeField] Animator _animator;

    [Header("Config")]
    [SerializeField] int _lives;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _hurtClip;

    private int _maxLives;

    public int Lives => _lives;
    public int MaxLives => _maxLives;

    public event Action OnDamaged;
    public event Action OnHeal;

    private bool _isBlocking;

    void Reset()
    {
        _lives = 3;
        _maxLives = _lives;
    }

    void Start()
    {
        _maxLives = _lives;
    }

    public void TakeDamage(int damageAmount)
    {
        if (_isBlocking)
            return;

        _lives -= damageAmount;

        _animator.SetTrigger("Hit");

        OnDamaged?.Invoke();

        if (_lives <= 0)
        {
            Die();
            _lives = 0;
        }
    }

    public void SetBlocking(bool blocking)
    {
        _isBlocking = blocking;
    }

    public void Die()
    {
        _animator.SetTrigger("Death");

        if (!GetComponentInParent<PlayerInput>())
        {
            Destroy(gameObject, 2.5f);
        }
        //else
        //{
        //    SceneManager.LoadScene(3);
        //}
    }
}