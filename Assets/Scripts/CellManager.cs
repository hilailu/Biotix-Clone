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

    [SerializeField] private GameObject miniCell;
    [SerializeField] private Camera main;

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

        if (transferred > startVal && damaged.value > 0)
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

    public void Attack2(Cell attacked)
    {
        foreach (Cell cell in selectedCells)
        {
            cell.Attack(attacked);
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
