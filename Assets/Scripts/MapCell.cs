using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum MapCellType
    {
        Normal = 1,
        Forest = 2,
        Mountain = 3,
        Sea = 9,
    }
    public class MapCell
    {
        public float g = 0;
        public float h = 0;
        public float f = 0;
        private int cellx = 0;
        public int CellX
        {
            get 
            {
                return cellx;
            }
            set
            {
                cellx = value;
            }
        }
        private int cellz = 0;
        public int CellZ
        {
            get
            {
                return cellz;
            }
            set
            {
                cellz = value;
            }
        }
        public SceneManager sceneManager;
        public MapCellType cellType = MapCellType.Normal;
        public MapCell parent;
        private MapCellItem item;
        public MapCell(SceneManager sceneManager, int x, int z)
        {
            this.cellx = x;
            this.cellz = z;
            this.sceneManager = sceneManager;
            CreateMapCellItem();
        }
        private void CreateMapCellItem()
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/MapCell"), GameObject.Find("Map").transform) as GameObject;
            obj.transform.position = new Vector3(this.cellx + 0.5f, 0f, this.cellz + 0.5f);
            obj.name = "MapCell[" + this.cellx + "," + this.cellz + "]";
            this.item = obj.GetComponent<MapCellItem>();
            this.item.SetData(this);
        }

        public void WalkToHere()
        {
            if ( cellType == MapCellType.Sea )
            {
                this.sceneManager.SetTips("请点击可行走区域！");
                return;
            }
            this.sceneManager.ReFind();
            this.sceneManager.WalkToHere(this.sceneManager.GetPlayer().CurCell, this);
        }

        public void ToNormal()
        {
            this.item.ToNormal();
        }

        public void ToWalk()
        {
            this.item.ToWalk();
        }

        public void ToSea()
        {
            this.item.ToSea();
        }
        public void ToMountain()
        {
            this.item.ToMountain();
        }
        public void ToForest()
        {
            this.item.ToForest();
        }
        public void SetCellType(MapCellType type)
        {
            this.cellType = type;
            if (type == MapCellType.Sea)
            {
                ToSea();
            }
            else if (type == MapCellType.Forest)
            {
                ToForest();
            }
            else if (type == MapCellType.Mountain)
            {
                ToMountain();
            }
            else
            {
                ToNormal();
            }
        }
        public void UpdateParentAndGF(MapCell parent, float g)
        {
            this.parent = parent;
            this.g = g;
            this.f = this.g + this.h;
        }
        public MapCellItem GetMapCellItem()
        {
            return item;
        }
        public void Reset()
        {
            this.g = 0;
            this.h = 0;
            this.f = 0;
            this.parent = null;
            SetCellType(this.cellType);
        }
        public void ShowPathNode()
        {
            ToWalk();
            if (parent != null)
            {
                parent.ShowPathNode();
            }
        }
    }
}
