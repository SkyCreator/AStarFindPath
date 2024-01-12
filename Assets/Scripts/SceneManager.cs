using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public int SceneWidth = 10;
    public int SceneHeight = 10;
    [SerializeField]
    private Transform PlayerTran;
    [SerializeField]
    private Transform TargetTran;
    [SerializeField]
    private Button RandBtn;
    [SerializeField]
    private Toggle Toggle1;
    [SerializeField]
    private Toggle Toggle2;
    [SerializeField]
    private Toggle Toggle3;
    [SerializeField]
    private Toggle CalcHToggle;
    [SerializeField]
    private Text Tips;
    private Player player;
    public MapCell[,] MapCellGroup;
    private int SeaCellNum = 50;
    private AStarFindPath astar = new AStarFindPath();
    private bool m_UseCellType = false;
    // Start is called before the first frame update
    void Start()
    {
        InitMapCell();
        PlayerTran.GetComponent<Renderer>().material.color = Color.yellow;
        TargetTran.GetComponent<Renderer>().material.color = Color.cyan;
        RandBtn.onClick.AddListener(OnRandBtnClick);
        Toggle1.onValueChanged.AddListener(isOn => { if (isOn) astar.SetCalcHType(CalcHType.Manhattan); });
        Toggle2.onValueChanged.AddListener(isOn => { if (isOn) astar.SetCalcHType(CalcHType.Diagonal); });
        Toggle3.onValueChanged.AddListener(isOn => { if (isOn) astar.SetCalcHType(CalcHType.Euclid); });
        CalcHToggle.onValueChanged.AddListener(isOn => { SetUseCellType(isOn); });
        astar.SetSceneManager(this);
        player = PlayerTran.GetComponent<Player>() as Player;
        player.CurCell = MapCellGroup[0, 0];
    }
    public Player GetPlayer()
    {
        return player;
    }
    private void OnRandBtnClick()
    {
        InitMapSeaCell();
    }

    private void InitMapCell()
    {
        MapCellGroup = new MapCell[SceneWidth, SceneHeight];
        for ( int i=0; i<SceneWidth; ++i )
        {
            for ( int j=0; j<SceneHeight; ++j )
            {
                MapCellGroup[i, j] = new MapCell(this, i, j);
                if (i==0 && j==0)
                {
                    MapCellGroup[i, j].SetCellType(MapCellType.Normal);
                }
                else
                {
                    MapCellType type = GetRandCellType();
                    MapCellGroup[i, j].SetCellType(type);
                }
            }
        }
    }
    private void InitMapSeaCell()
    {
        for (int i = 0; i < SceneWidth; ++i)
        {
            for (int j = 0; j < SceneHeight; ++j)
            {
                if (player.CurCell.CellX == i && player.CurCell.CellZ == j)
                {
                    continue;
                }
                MapCellType type = GetRandCellType();
                MapCellGroup[i, j].SetCellType(type);
            }
        }
    }
    public void SetTargetPos(int x, int z)
    {
        this.TargetTran.position = new Vector3(x + 0.5f, 0f, z + 0.5f);
    }
    public void WalkToHere(MapCell start, MapCell end)
    {
        astar.WalkToHere(start, end);
        if (end.parent == null)
        {
            SetTips("该位置不可到达！");
            return;
        }
        ShowPath(end);
        SetTargetPos(end.CellX, end.CellZ);
    }
    private MapCellType GetRandCellType()
    {
        int rand = Random.Range(0, 10);
        if (rand <= 4)
        {
            return MapCellType.Normal;
        }
        else if (rand == 5)
        {
            return MapCellType.Forest;
        }
        else if (rand == 6)
        {
            return MapCellType.Mountain;
        }
        else
        {
            return MapCellType.Sea;
        }
    }
    public void ReFind()
    {
        foreach (MapCell mc in MapCellGroup)
        {
            mc.Reset();
        }
    }
    private void ShowPath(MapCell cell)
    {
        cell.ShowPathNode();
        player.WalkToHere(cell);
    }
    public void SetUseCellType(bool isUse)
    {
        m_UseCellType = isUse;
    }
    public bool GetUseCellType()
    {
        return m_UseCellType;
    }
    public void SetTips(string tips)
    {
        Tips.text = tips;
        StartCoroutine(HideTips());
    }
    private IEnumerator HideTips()
    {
        yield return new WaitForSeconds(3f);
        Tips.text = "";
    }
}
