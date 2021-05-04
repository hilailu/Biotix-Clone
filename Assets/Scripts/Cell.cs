using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class Cell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public uint value;

    [SerializeField] private uint maxvalue;
    [SerializeField] private bool isCaptured;

    private bool isAddCoroutineRunning = false;
    private bool isRemoveCoroutineRunning = false;

    private TMP_Text val;
    private LineRenderer lineRend;
    private Vector2 startPos;
    private Vector2 endPos;

    private void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        val = GetComponent<TMP_Text>();

        if (maxvalue == 50)
            value = 0;
        else
            value = maxvalue;

        ValueText();

        bool result = value == 60 ? isCaptured = true : isCaptured = false;
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
            val.text = value.ToString();
        else
            val.text = string.Empty;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>() != null)
            Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);

        endPos = eventData.pointerCurrentRaycast.worldPosition;
        lineRend.SetPosition(0, new Vector3(startPos.x, startPos.y, 0f));
        lineRend.SetPosition(1, new Vector3(endPos.x, endPos.y, 0f));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lineRend.positionCount = 2;
        startPos = eventData.pointerCurrentRaycast.worldPosition;
        Debug.Log("You started dragging. Start pos: " + startPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();
        if (cell == null)
            lineRend.positionCount = 0;
        else
        {
            uint transferred = Convert.ToUInt32(Math.Floor((float)value / 2));
            cell.value +=  transferred;
            this.value -= transferred;
        }
        Debug.Log("You ended a drag");
    }



    private IEnumerator AddRoutine()
    {
        isAddCoroutineRunning = true;
        for (uint i = value; i < maxvalue; i++)
        {
            value++;
            ValueText();
            yield return new WaitForSeconds(1f);
        }
        isAddCoroutineRunning = false;
    }

    private IEnumerator RemoveRoutine()
    {
        isRemoveCoroutineRunning = true;
        for (uint i = value; i > maxvalue; i = i - 2)
        {
            value = value - 2;
            ValueText();
            yield return new WaitForSeconds(0.5f);
        }
        isRemoveCoroutineRunning = false;
    }

}
