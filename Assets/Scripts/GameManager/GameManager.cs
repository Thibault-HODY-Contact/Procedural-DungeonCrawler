using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance => _instance;

    public int numberlevel = 0;

    [SerializeField] public GameObject genGenerator = null;

    public bool hasGenerate = false;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(!hasGenerate && genGenerator)
        {
            genGenerator.GetComponent<GenGenerator>().levelCount = numberlevel;
            genGenerator.GetComponent<GenGenerator>().GenerateLevels();
            hasGenerate = true;
        }
    }


}
