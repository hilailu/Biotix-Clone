using System.Collections;
using System.Linq;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private System.Random r;

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

            if (linq == null)
                break;

            // random Bot cell
            var cell = linq.ElementAt(r.Next(0, linq.Count()));

            // linq targets 
            var linqTarget = CellManager.instance.cells.Where(w => !(w.owner == Owner.Bot && w.value >= w.maxValue));

            // random target from linq
            var target = linqTarget.ElementAt(r.Next(0, linqTarget.Count()));

            cell.Attack(target);
        }
    }
}
