using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum CalcHType
    {
        Manhattan,  //曼哈顿距离
        Euclid,     //欧几里得距离
        Diagonal,   //对角线距离
    }
    public class AStarFindPath
    {
        private CalcHType m_CalcHType = CalcHType.Diagonal;
        private SceneManager m_SceneManager = null;
        
        public void SetCalcHType(CalcHType type)
        {
            m_CalcHType = type;
        }
        public void SetSceneManager(SceneManager manager)
        {
            m_SceneManager = manager;
        }
        public void WalkToHere(MapCell start, MapCell end)
        {
            List<MapCell> openList = new List<MapCell>();
            List<MapCell> closeList = new List<MapCell>();
            CalcMapCellF(start, end);
            openList.Add(start);
            while (openList.Count > 0)
            {
                MapCell minCostCell = FindMinCostMapCell(openList);
                openList.Remove(minCostCell);
                closeList.Add(minCostCell);
                List<MapCell> neighborList = FindNeighbor(minCostCell, closeList);
                foreach (MapCell cell in neighborList)
                {
                    if ( openList.Contains(cell) )
                    {
                        float curG = PreCalcG(cell, minCostCell);
                        if (curG < cell.g)
                        {
                            cell.UpdateParentAndGF(minCostCell, curG);
                        }
                    }
                    cell.parent = minCostCell;
                    CalcMapCellF(cell, end);
                    if (!openList.Contains(cell))
                    {
                        openList.Add(cell);
                    }
                }
                if (openList.Contains(end))
                {
                    break;
                }
            }
        }
        public MapCell FindMinCostMapCell(List<MapCell> list)
        {
            MapCell minCell = null;
            float minF = 99999f;
            for ( int i=0; i<list.Count; ++i )
            {
                if (list[i].f < minF)
                {
                    minF = list[i].f;
                    minCell = list[i];
                }
            }
            return minCell;
        }
        public List<MapCell> FindNeighbor(MapCell cell, List<MapCell> closeList)
        {
            int width = m_SceneManager.SceneWidth;
            int height = m_SceneManager.SceneHeight;
            MapCell[,] MapCellGroup = m_SceneManager.MapCellGroup;
            List<MapCell> neighborList = new List<MapCell>();
            //上
            if (cell.CellZ + 1 < height && MapCellGroup[cell.CellX, cell.CellZ + 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX, cell.CellZ + 1]);
            }
            //下
            if (cell.CellZ - 1 >= 0 && MapCellGroup[cell.CellX, cell.CellZ - 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX, cell.CellZ - 1]);
            }
            //左
            if (cell.CellX - 1 >= 0 && MapCellGroup[cell.CellX - 1, cell.CellZ].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX - 1, cell.CellZ]);
            }
            //右
            if (cell.CellX + 1 < width && MapCellGroup[cell.CellX + 1, cell.CellZ].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX + 1, cell.CellZ]);
            }
            //左上
            if (cell.CellX - 1 >= 0 && cell.CellZ + 1 < height && MapCellGroup[cell.CellX - 1, cell.CellZ + 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX - 1, cell.CellZ + 1]);
            }
            //右上
            if (cell.CellX + 1 < width && cell.CellZ + 1 < height && MapCellGroup[cell.CellX + 1, cell.CellZ + 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX + 1, cell.CellZ + 1]);
            }
            //左下
            if (cell.CellX - 1 >= 0 && cell.CellZ - 1 >= 0 && MapCellGroup[cell.CellX - 1, cell.CellZ - 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX - 1, cell.CellZ - 1]);
            }
            //右下
            if (cell.CellX + 1 < width && cell.CellZ - 1 >= 0 && MapCellGroup[cell.CellX + 1, cell.CellZ - 1].cellType != MapCellType.Sea)
            {
                neighborList.Add(MapCellGroup[cell.CellX + 1, cell.CellZ - 1]);
            }
            foreach (MapCell mc in closeList)
            {
                if (neighborList.Contains(mc))
                {
                    neighborList.Remove(mc);
                }
            }
            return neighborList;
        }
        private float CalcMapCellG(MapCell cell)
        {
            if (cell.parent != null)
            {
                bool bStraight = (cell.parent.CellX == cell.CellX || cell.parent.CellZ == cell.CellZ);
                cell.g = cell.parent.g + GetCost(cell, bStraight);
            }
            else
            {
                cell.g = 0;
            }
            return cell.g;
        }
        private float CalcMapCellH(MapCell cell, MapCell end)
        {
            int dx = Math.Abs(cell.CellX - end.CellX);
            int dz = Math.Abs(cell.CellZ - end.CellZ);
            //曼哈顿距离
            if (m_CalcHType == CalcHType.Manhattan)
            {
                cell.h = dx + dz;
            }
            //欧式距离
            else if ( m_CalcHType == CalcHType.Euclid )
            {
                cell.h = (float)(10 * Math.Sqrt(dx * dx + dz * dz));
            }
            //对角线估价
            else
            {
                cell.h = 10 * (dx + dz) + (1.414f*10 - 2 * 10) * Math.Min(dx, dz);
            }
            return cell.h;
        }
        private float CalcMapCellF(MapCell cell, MapCell end)
        {
            cell.f = CalcMapCellG(cell) + CalcMapCellH(cell, end);
            return cell.f;
        }
        public float GetCost(MapCell cell, bool bStraight)
        {
            return (bStraight ? GetCostD(cell) : GetCostD2(cell));
            
        }
        private float GetFactor(MapCell cell)
        {
            return m_SceneManager.GetUseCellType() ? (float)cell.cellType:1;
        }
        private float GetCostD(MapCell cell)
        {
            return 10 * GetFactor(cell);
        }
        private float GetCostD2(MapCell cell)
        {
            return 10*1.414f * GetFactor(cell);
        }
        private float PreCalcG(MapCell cell, MapCell parent)
        {
            float G;
            if (parent != null)
            {
                bool bStraight = (parent.CellX == cell.CellX || parent.CellZ == cell.CellZ);
                G = parent.g + GetCost(cell, bStraight);
            }
            else
            {
                G = 0;
            }
            return G;
        }
    }
}
