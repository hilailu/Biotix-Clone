using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void Pause(bool set)
    {
        if (set) 
            Time.timeScale = 0;
        else 
            Time.timeScale = 1;
    }

    public void Load(int level)
        => SceneManager.LoadScene(level);
}
