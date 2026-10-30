using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LifeScript : MonoBehaviour, IDamageable
{
    [Header("Dependencies")]
    [SerializeField] Animator _animator;

    [Header("Config")]
    [SerializeField] private int _lives = 3;
    [SerializeField] private int damagePerHeart = 10;

    private bool hasDied = false;
    private int _maxLives;
    private int _currentHealth;
    private int _maxHealth;
    public int Lives => _lives;
    public int MaxLives => _maxLives;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _hurtClip;

    [Header("Gas Damage")]
    [SerializeField] private GasSpread gas;
    [SerializeField] private Transform playerPosition;
    [SerializeField] private int gasDamage = 1;
    [SerializeField] private float gasDamageInterval = 1f;

    private float gasDamageTimer;

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

        _maxHealth = _maxLives * damagePerHeart;
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        if (gas == null || playerPosition == null)
            return;

        Vector3 playerPos = playerPosition.position;
        Vector3 gasPos = gas.transform.position;

        float distance = Vector2.Distance(
            new Vector2(playerPos.x, playerPos.z),
            new Vector2(gasPos.x, gasPos.z)
        );

        if (distance <= gas.CurrentRadius)
        {
            gasDamageTimer += Time.deltaTime;

            if (gasDamageTimer >= gasDamageInterval)
            {
                TakeDamage(gasDamage);
                gasDamageTimer = 0f;
            }
        }
        else
        {
            gasDamageTimer = 0f;
        }
        _animator.SetInteger("vies", _lives);

        if (!hasDied && _currentHealth <= 0)
        {
            hasDied = true;
        }

        if (hasDied)
        {
            AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("death") && state.normalizedTime >= 1f)
            {
                SceneManager.LoadScene(2);
            }
        }
    }
    public void TakeDamage(int damageAmount)
    {
        if (_isBlocking || _currentHealth <= 0)
            return;

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Max(0, _currentHealth);

        _lives = Mathf.CeilToInt(
            (float)_currentHealth / damagePerHeart
        );

        _animator.SetTrigger("Hit");
        OnDamaged?.Invoke();

        if (_currentHealth <= 0)
        {
            _lives = 0;
            Die();
        }
    }
    public bool Heal(int healAmount)
    {
        if (_currentHealth <= 0 || _currentHealth >= _maxHealth)
            return false;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + healAmount);
        _lives = Mathf.CeilToInt((float)_currentHealth / damagePerHeart);

        OnHeal?.Invoke();
        return true;
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
            transform.root.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        //else
        //{
        //    SceneManager.LoadScene(3);
        //}
    }
}