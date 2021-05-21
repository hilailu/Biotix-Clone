using UnityEngine;

public class CellCenter : MonoBehaviour
{
    private SpriteRenderer rend;
    private Color[] colors = new Color[4] { Color.magenta, Color.red, Color.yellow, Color.green };

    private void Awake()
        => rend = GetComponent<SpriteRenderer>();

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
                if (CellManager.instance.botColor == Color.clear)
                {
                    CellManager.instance.botColor = colors[Random.Range(0, colors.Length)];
                    rend.color = CellManager.instance.botColor;
                }
                else rend.color = CellManager.instance.botColor;
                break;
        }
    }
}
