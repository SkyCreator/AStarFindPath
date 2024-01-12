using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Stack<MapCell> stPath = new Stack<MapCell>();
    private MapCell curCell;
    public MapCell CurCell
    {
        get
        {
            return curCell;
        }
        set
        {
            curCell = value;
        }
    }
    public void WalkToHere(MapCell cell)
    {
        GetPath(cell);
        MapCell mc = stPath.Pop();
        StartCoroutine(WalkToCell(mc));
    }
    private IEnumerator WalkToCell(MapCell cell)
    {
        this.transform.position = cell.GetMapCellItem().transform.position;
        yield return new WaitForSeconds(0.3f);
        this.curCell = cell;
        cell.Reset();
        if (stPath.Count > 0)
        {
            MapCell mc = stPath.Pop();
            if (mc != null)
            {
                StartCoroutine(WalkToCell(mc));
            }
        }

    }
    //将路径结点压栈
    public void GetPath(MapCell cell)
    {
        stPath.Push(cell);
        if (cell.parent != null)
        {
            GetPath(cell.parent);
        }
    }   
}
