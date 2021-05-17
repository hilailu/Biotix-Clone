using UnityEngine;
using UnityEngine.SceneManagement;

public class CellCenter : MonoBehaviour
{
    private SpriteRenderer rend;
    private int level;

    private void Awake()
    {
        level = SceneManager.GetActiveScene().buildIndex;
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Owner owner)
    {
        switch (owner)
        {
            case Owner.None:
                rend.color = Color.white;
                break;

            case Owner.Player:
                rend.color = Color.cyan;
                break;

            case Owner.Bot:

                switch (level)
                {
                    case 1:
                        rend.color = Color.yellow;
                        break;

                    case 2:
                        rend.color = Color.red;
                        break;

                    case 3:
                        rend.color = Color.green;
                        break;
                }
                break;
        }
    }
}
