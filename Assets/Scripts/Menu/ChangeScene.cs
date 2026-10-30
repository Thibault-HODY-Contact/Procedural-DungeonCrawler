using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _firstSelectedButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
    public void ChangeScenes(string name)
    {
        SceneManager.LoadScene(name);
    }
}
