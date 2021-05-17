using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bot : MonoBehaviour
{
    private List<Cell> selected = new List<Cell>();

    void Start()
    {
        StartCoroutine(BotRoutine());
    }

    private void SelectCells()
    {
        ExecuteEvents.Execute<IPointerClickHandler>(CellManager.instance.botCells[0].gameObject, null, null);
    }

    private IEnumerator BotRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (CellManager.instance.botCells.Count > 0)
        {
            yield return new WaitForSeconds(2f);
            ExecuteEvents.Execute<IPointerClickHandler>(CellManager.instance.botCells[0].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
    }
}
