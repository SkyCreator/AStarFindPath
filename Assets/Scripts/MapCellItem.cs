using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCellItem : MonoBehaviour
{
    public GameObject self;
    private MapCell mapCell;

    void OnMouseUpAsButton()
    {
        mapCell.WalkToHere();
    }
    public void SetData(MapCell mapCell)
    {
        this.mapCell = mapCell;
    }
    public void ToNormal()
    {
        self.GetComponent<Renderer>().material.color = Color.white;
    }

    public void ToWalk()
    {
        self.GetComponent<Renderer>().material.color = Color.green;
    }

    public void ToSea()
    {
        self.GetComponent<Renderer>().material.color = mapCell.sceneManager.GetUseCellType() ? Color.blue : Color.black;
    }

    public void ToForest()
    {
        self.GetComponent<Renderer>().material.color = mapCell.sceneManager.GetUseCellType() ? new Color(0.02f, 0.31f, 0.05f) : Color.white;
    }

    public void ToMountain()
    {
        self.GetComponent<Renderer>().material.color = mapCell.sceneManager.GetUseCellType() ? Color.yellow : Color.white;
    }
}
