using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.UI;
using System.Linq;

public enum Owner
{
    None,
    Player,
    Bot,
}

public class Cell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int value;
    public Owner owner;
    public LineRenderer lineRend;
    public SpriteRenderer selectRing;

    [SerializeField] private int maxvalue;

    private CellColor cellColor;

    private bool isAddCoroutineRunning = false;
    private bool isRemoveCoroutineRunning = false;

    private TMP_Text val;
    private Vector2 startPos;
    private Vector2 endPos;

    private float UniqueId { get; set; }

    public float GetId()
        => this.UniqueId;

    private void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        val = GetComponent<TMP_Text>();
        cellColor = GetComponentInChildren<CellColor>();

        UniqueId = transform.position.sqrMagnitude;

        CellManager.instance.cells.Add(this);

        ValueText();
        cellColor.SetColor(owner);
    }

    private void Update()
    {
        //if (isAddCoroutineRunning) return;

        ValueText();

        //if (isCaptured)
        {
            if (value == maxvalue) return;

            if (value < maxvalue)
                if (!isAddCoroutineRunning)
                    StartCoroutine(AddRoutine());

            if (value > maxvalue)
                if (!isRemoveCoroutineRunning)
                    StartCoroutine(RemoveRoutine());
        }
    }

    private void ValueText()
    {
        if (value != 0)
            val.text = Math.Abs(value).ToString();
        else
            val.text = string.Empty;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if (cell != null)
            Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);

        if (startPos != Vector2.zero)
        {
            endPos = eventData.pointerCurrentRaycast.worldPosition;
            lineRend.SetPosition(0, new Vector3(startPos.x, startPos.y, 0f));
            lineRend.SetPosition(1, new Vector3(endPos.x, endPos.y, 0f));

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
            lineRend.positionCount = 2;
            startPos = eventData.pointerCurrentRaycast.worldPosition;
            Debug.Log("You started dragging. Start pos: " + startPos);
        }
        else
        {
            lineRend.positionCount = 0;
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
                int startVal = cell.value;

                int transferred = 0;

                foreach (Cell item in CellManager.instance.selectedCells)
                {
                    int cellTransfer = Convert.ToInt32(Math.Floor((float)item.value / 2));
                    transferred += cellTransfer;
                    item.value -= cellTransfer;
                }

                if (cell.owner == Owner.Bot)
                {
                    if (transferred >= cell.value)
                    {
                        var diff = transferred - cell.value;
                        cell.value = diff;
                    }
                    else
                        cell.value -= transferred;
                }
                else cell.value += transferred;

                if (cell.value > 0 && transferred > startVal)
                {
                    cell.owner = this.owner;
                    cell.GetComponentInChildren<CellColor>().SetColor(cell.owner);
                }
                if (cell.value == 0)
                    cell.owner = Owner.None;


                foreach (Cell item in CellManager.instance.selectedCells)
                {
                    item.selectRing.enabled = false;
                    item.lineRend.positionCount = 0;
                    item.lineRend.positionCount = 2;
                }
                CellManager.instance.selectedCells.Clear();
            }
            
            if (CellManager.instance.selectedCells.Count() > 0)
            {
                foreach (Cell item in CellManager.instance.selectedCells)
                {
                    item.lineRend.positionCount = 0;
                    item.lineRend.positionCount = 2;
                }
            }
            
            Debug.Log("You ended a drag");
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
                ValueText();
            }
            isAddCoroutineRunning = false;
        }
    }

    private IEnumerator RemoveRoutine()
    {
        isRemoveCoroutineRunning = true;
        for (int i = value; i > maxvalue; i = i - 2)
        {         
            yield return new WaitForSeconds(1f);
            value = value - 2;
            ValueText();
        }
        isRemoveCoroutineRunning = false;
    }

}
