using System.Collections.Generic;
using UnityEngine;

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

  //  public List<Cell> cells = new List<Cell>();
    public List<Cell> selectedCells = new List<Cell>();
    public List<Cell> botCells = new List<Cell>();

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
