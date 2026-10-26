using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject _pauseMenu;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject volumePanel;

    [Header("First Selected")]
    [SerializeField] private GameObject _firstPauseSelected;
    [SerializeField] private GameObject _firstVolumeSelected;

    public void Pause()
    {
        pausePanel.SetActive(true);
        volumePanel.SetActive(false);

        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstPauseSelected);
    }

    public void Resume()
    {
        volumePanel.SetActive(false);
        pausePanel.SetActive(true);

        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void OpenVolume()
    {
        pausePanel.SetActive(false);
        volumePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstVolumeSelected);
    }

    public void CloseVolume()
    {
        volumePanel.SetActive(false);
        pausePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstPauseSelected);
    }
}