using UnityEngine;

[RequireComponent(typeof(CellCenter))]
public class MiniCell : MonoBehaviour
{
    public Owner owner;

    [SerializeField] private int value;
    private float speed = 1f;
    private Cell to;
    private bool isMoving;

    public void Move(Cell to)
    {
        GetComponent<CellCenter>().SetColor(this.owner);
        isMoving = true;
        this.to = to;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, this.to.transform.position, speed * Time.deltaTime);

            if (transform.position == this.to.transform.position)
            {
                isMoving = false;
                to.IncreaseAmount(this.owner, this.value);
                Destroy(this.gameObject);
            }
        }
    }
}
