using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("sdfdsgdfsdfgdfgdfgdfgdfgdfg");
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(3);
        }
    }
}