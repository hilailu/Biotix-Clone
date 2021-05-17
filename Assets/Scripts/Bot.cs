using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bot : MonoBehaviour
{
    private List<Cell> selected = new List<Cell>();
    System.Random r;

    private void Awake()
    {
        r = new System.Random();
        StartCoroutine(BotRoutine());
    }

    private IEnumerator BotRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return new WaitForSeconds(2f);
            // Bot cells
            var linq = CellManager.instance.cells.Where(w => w.owner == Owner.Bot);
            // random Bot cell
            if (linq.Count() == 0) 
            {
                Debug.Log("Bot lost");
                break;
            }
            var cell = linq.ElementAt(r.Next(0, linq.Count()));
            // linq targets 
            var linqTarget = CellManager.instance.cells.Where(w => !(w.owner == Owner.Bot && w.value == w.maxValue));
            // random target from linq
            var target = linqTarget.ElementAt(r.Next(0, linqTarget.Count()));
            cell.Attack(target);
            Debug.Log(cell + " attacking " + target);
        }
    }
}
