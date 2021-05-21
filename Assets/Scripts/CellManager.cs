using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using TMPro;
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
        botColor = Color.clear;
        OnOwnerChanged += CheckGameOver;
    }
    #endregion

    [SerializeField] private AdsManager am;
    [SerializeField] private GameObject endGame;
    [SerializeField] private Button next;

    public Color botColor;

    public Action OnOwnerChanged;

    public List<Cell> cells = new List<Cell>();
    public List<Cell> selectedCells = new List<Cell>();

    private void Update()
    {
        foreach (Cell cell in cells)
        {
            cell.CheckValue();
        }
    }

    private void OnDestroy()
        => OnOwnerChanged -= CheckGameOver;

    private void CheckGameOver()
    {
        var linqPlayer = cells.Where(w => w.owner == Owner.Player);
        if (linqPlayer.Count() == 0)
        {
            endGame.SetActive(true);
            endGame.GetComponentInChildren<TMP_Text>().text = "YOU LOST";
            next.interactable = false;
            am.ShowAd();
            Debug.Log("Player Lost");
        }

        var linqBot = cells.Where(w => w.owner == Owner.Bot);
        if (linqBot.Count() == 0)
        {
            endGame.SetActive(true);
            endGame.GetComponentInChildren<TMP_Text>().text = "YOU WON";
            next.interactable = true;
            am.ShowAd();
            Debug.Log("Bot Lost");
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
