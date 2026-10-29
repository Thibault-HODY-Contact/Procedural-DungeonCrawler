using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class numberLevelChoose : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    
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
