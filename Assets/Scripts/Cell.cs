using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public enum Owner
{
    None,
    Player,
    Bot,
}

public class Cell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int value;
    public Owner owner;
    public LineRenderer lineRend;
    public SpriteRenderer selectRing;

    [SerializeField] private int maxvalue;

    private CellCenter cellColor;
    private TMP_Text val;
    private Vector2 startPos;
    private Vector2 endPos;
    private Coroutine coAdd, coRem;

    private bool isAddCoroutineRunning = false;
    private bool isRemoveCoroutineRunning = false;

    private void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        val = GetComponent<TMP_Text>();
        cellColor = GetComponentInChildren<CellCenter>();

        if (this.owner == Owner.Bot)
            CellManager.instance.botCells.Add(this);

        ValueText();
        cellColor.SetColor(owner);
    }

    private void Update()
    {
        ValueText();

        if (owner != Owner.None)
        {
            if (value == maxvalue) return;

            if (value < maxvalue && !isAddCoroutineRunning)
            {
                if (coRem != null)
                {
                    StopCoroutine(coRem);
                    isRemoveCoroutineRunning = false;
                }
                coAdd = StartCoroutine(AddRoutine());
            }

            if (value > maxvalue && !isRemoveCoroutineRunning)
            {
                if (coAdd != null)
                {
                    StopCoroutine(coAdd);
                    isAddCoroutineRunning = false;
                }
                coRem = StartCoroutine(RemoveRoutine());
            }
        }
    }

    private void ValueText()
    {
        if (value == 0)
        {
            val.text = string.Empty;

            if (this.owner != Owner.None)
            {
                this.owner = Owner.None;
                cellColor.SetColor(this.owner);
            }
        }
        else
            val.text = Math.Abs(value).ToString();
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject?.GetComponent<Cell>();

        if (startPos != Vector2.zero)
        {
            endPos = eventData.pointerCurrentRaycast.worldPosition;

            if (cell != null && cell.owner == Owner.Player && !CellManager.instance.selectedCells.Contains(cell))
            {
                cell.selectRing.enabled = true;
                CellManager.instance.selectedCells.Add(cell);
            }

            foreach (Cell item in CellManager.instance.selectedCells)
            {
                item.lineRend.SetPosition(0, new Vector3(item.transform.position.x, item.transform.position.y, 0f));
                item.lineRend.SetPosition(1, new Vector3(endPos.x, endPos.y, 0f));
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();

        if (cell != null && cell.owner == Owner.Player)
        {
            cell.selectRing.enabled = true;
            cell.lineRend.positionCount = 2;
            startPos = eventData.pointerCurrentRaycast.worldPosition;
            Debug.Log("You started dragging. Start pos: " + startPos);
        }
        else if (cell != null)
        {
            cell.lineRend.positionCount = 0;
            startPos = Vector2.zero;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (startPos != Vector2.zero)
        {
            var cell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();

            if (cell != null)
            {
                CellManager.instance.Attack2(cell);
            }

            CellManager.instance.Clear();
            Debug.Log("You ended a drag");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Cell touchedCell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if (touchedCell.owner == Owner.Player)
        {
            if (!CellManager.instance.selectedCells.Contains(touchedCell))
            {
                CellManager.instance.selectedCells.Add(touchedCell);
                touchedCell.selectRing.enabled = true;
                Debug.Log("Cell selected");
            }
            else if (CellManager.instance.selectedCells.Contains(touchedCell) && CellManager.instance.selectedCells.Count > 1)
            {
                CellManager.instance.Attack2(touchedCell);
                CellManager.instance.Clear();
                Debug.Log("Attacking your own cell");
            }
        }
        if (touchedCell.owner != Owner.Player && CellManager.instance.selectedCells.Count > 0)
        {
            CellManager.instance.Attack2(touchedCell);
            CellManager.instance.Clear();
            Debug.Log("Attacking different cell");
        }
    }

    public void Attack(Cell attacked)
    {
        if (this != attacked)
        {
            int transferred = Convert.ToInt32(Math.Floor((float)this.value / 2));
            int amountOfMiniCells = Convert.ToInt32(Math.Ceiling((float)transferred / 2));

            this.value -= transferred;

            for (int i = 0; i < amountOfMiniCells; i++)
            {
                Vector3 rand = this.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0);
                GameObject mc = Instantiate((GameObject)Resources.Load("MiniCell"), rand, Quaternion.identity, this.transform);
                MiniCell miniCell = mc.GetComponent<MiniCell>();
                if (i == amountOfMiniCells - 1 && transferred % 2 == 1)
                    miniCell.value = 1;
                else
                    miniCell.value = 2;
                miniCell.owner = this.owner;
                miniCell.Move(attacked);
            }
        }
    }

    public void IncreaseAmount(Owner owner, int val)
    {
        var startVal = this.value;

        if (this.owner != Owner.None && owner != this.owner)
            this.value -= val;
        else
            this.value += val;

        if ((this.owner == Owner.None && this.value > 0) || (this.owner != Owner.None && this.value < 0))
        {
            if (CellManager.instance.botCells.Contains(this))
                CellManager.instance.botCells.Remove(this);
            this.owner = owner;
            this.cellColor.SetColor(owner);
            if (this.owner == Owner.Bot && !CellManager.instance.botCells.Contains(this))
                CellManager.instance.botCells.Add(this);
        }
    }

    private IEnumerator AddRoutine()
    {
        if (owner != Owner.None)
        {
            isAddCoroutineRunning = true;
            for (int i = value; i < maxvalue; i++)
            {               
                yield return new WaitForSeconds(1f);
                value++;
            }
            isAddCoroutineRunning = false;
        }
    }

    private IEnumerator RemoveRoutine()
    {
        isRemoveCoroutineRunning = true;
        for (int i = value; i > maxvalue; i -= 2)
        {         
            yield return new WaitForSeconds(1f);
            value -= 2;
        }
        isRemoveCoroutineRunning = false;
    }

}
