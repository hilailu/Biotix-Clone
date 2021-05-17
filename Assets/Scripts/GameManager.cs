using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;

    void Start()
    {
        level = SceneManager.GetActiveScene().buildIndex;
    }

    public void Pause(bool set)
    {
        if (set) 
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
