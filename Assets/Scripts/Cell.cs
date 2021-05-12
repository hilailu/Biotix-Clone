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

    private Coroutine coAdd, coRem;

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
        if (value != 0)
            val.text = Math.Abs(value).ToString();
        else
            val.text = string.Empty;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject?.GetComponent<Cell>();
        if (cell != null)
            Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);

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
                CellManager.instance.Attack(this.owner, cell);
            }

            CellManager.instance.Clear();
            
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
        }
        isRemoveCoroutineRunning = false;
    }

}
