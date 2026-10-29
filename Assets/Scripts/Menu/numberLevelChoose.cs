using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class numberLevelChoose : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;

    [Header("UI")]
    [SerializeField] private GameObject _firstSelectedButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
    public void chooseValue()
    {
        string selectedText = dropdown.options[dropdown.value].text;
        if (int.TryParse(selectedText, out int levelNumber))
        {
            GameManager.Instance.numberlevel = levelNumber;
        }
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
