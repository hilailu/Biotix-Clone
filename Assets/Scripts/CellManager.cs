using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellManager : MonoBehaviour
{
    #region Singleton
    public static CellManager instance;
    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    #endregion

    public List<Cell> cells = new List<Cell>();
    public List<Cell> selectedCells = new List<Cell>();

    private Vector2 touchPosition;
    private EventSystem system;

    [SerializeField] private Camera main;

    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;

            var pointerEventData = new PointerEventData(system) { position = touchPosition };
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.CompareTag("Cell"))
                    {
                        Cell touchedCell = result.gameObject.GetComponent<Cell>();
                        if (touchedCell.owner == Owner.Player)
                        {
                            if (!selectedCells.Contains(touchedCell))
                            {
                                selectedCells.Add(touchedCell);
                                touchedCell.selectRing.enabled = true;
                                Debug.Log("Cell selected");
                            }
                            else if (selectedCells.Contains(touchedCell) && selectedCells.Count > 1)
                            {
                                Attack(selectedCells[0].owner, touchedCell);
                                Clear();
                                Debug.Log("Attacking your own cell");
                            }
                        }
                        if (touchedCell.owner != Owner.Player && selectedCells.Count > 0)
                        {
                            Attack(selectedCells[0].owner, touchedCell);
                            Clear();
                            Debug.Log("Attacking different cell");
                        }
                        return;
                    }

                    else if (result.gameObject.CompareTag("BG") && selectedCells.Count > 0)
                    {

                        foreach (Cell cell in selectedCells)
                        {
                            cell.selectRing.enabled = false;
                        }
                        selectedCells.Clear();

                        Debug.Log("you touched background and cleared selected cells");
                    }
                }
            }
            
        }
    }

    public void Attack(Owner owner, Cell damaged)
    {
        int startVal = damaged.value;

        int transferred = 0;

        foreach (Cell item in selectedCells)
        {
            int cellTransfer = Convert.ToInt32(Math.Floor((float)item.value / 2));
            transferred += cellTransfer;
            item.value -= cellTransfer;
        }

        if (damaged.owner == Owner.Bot)
        {
            if (transferred >= damaged.value)
            {
                var diff = transferred - damaged.value;
                damaged.value = diff;
            }
            else
                damaged.value -= transferred;
        }
        else damaged.value += transferred;

        var cellColor = damaged.GetComponentInChildren<CellColor>();

        if (damaged.value > 0 && transferred > startVal)
        {
            damaged.owner = owner;
            cellColor.SetColor(damaged.owner);
        }
        if (damaged.value == 0)
        {
            damaged.owner = Owner.None;
           cellColor.SetColor(damaged.owner);
        }
    }

    public void Clear()
    {
        if (selectedCells.Count > 0)
        {
            foreach (Cell item in selectedCells)
            {
                item.selectRing.enabled = false;
                item.lineRend.positionCount = 0;
                item.lineRend.positionCount = 2;
            }

            selectedCells.Clear();
        }
    }
}
