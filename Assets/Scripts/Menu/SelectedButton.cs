using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _firstSelectedButton;

    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
}