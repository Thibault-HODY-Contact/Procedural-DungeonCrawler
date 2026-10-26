using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILifeScript : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] List<Image> _imagesHeart;
    [SerializeField] LifeScript _lifeScript;

    private void Reset()
    {
        _lifeScript = GetComponent<LifeScript>();
    }

    void Start()
    {
        foreach (var image in _imagesHeart)
        {
            image.enabled = false;
        }

        UpdateUI();

        _lifeScript.OnDamaged += UpdateUI;
        _lifeScript.OnHeal += UpdateUI;
    }

    private void OnDestroy()
    {
        _lifeScript.OnDamaged -= UpdateUI;
        _lifeScript.OnHeal -= UpdateUI;
    }

    void Update()
    {
    }

    void UpdateUI()
    {
        int currentHP = _lifeScript.Lives;
        int maxHP = _lifeScript.MaxLives;

        if (currentHP < 0)
            return;

        for (int i = 0;  i < currentHP; i++)
        {
            _imagesHeart[i].enabled = true;
        }
        for (int i = currentHP; i < maxHP; i++)
        {
            _imagesHeart[i].enabled = false;
        }
    }
}