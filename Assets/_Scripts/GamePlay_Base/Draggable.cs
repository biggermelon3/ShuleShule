using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private GameManager GM;
    private bool placed = false;

    private Dictionary<Block, Vector3> allBlockCoordsDict = new Dictionary<Block, Vector3>();
    public List<Vector2> allXY = new List<Vector2>();

    public List<int> tempZs = new List<int>();


    void Start()
    {
        mainCamera = Camera.main; // »ñÈ¡Ö÷Ïà»ú
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        EventManager.OnRotateDraggable.AddListener(RotateThis);
    }

    private void RotateThis()
    {
        if (!placed)
        {
            transform.Rotate(0, 0, 90, Space.World);
        }
    }

    //get mousePos
    private Vector3 GetMouseWorldPos()
    {
        // ½«Êó±êÔÚÆÁÄ»ÉÏµÄÎ»ÖÃ×ª»»ÎªÔÚÊÀ½çÖÐµÄÎ»ÖÃ
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(gameObject.transform.position).z; // È·±£ÎïÌåÑØ×ÅZÖá²»ÒÆ¶¯
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDown()
    {
        InitializeBlockDict();
        if (placed)
        {
            return;
        }
       
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (placed)
        {
            return;
        }
        //if (Input.GetMouseButtonDown(1) && !placed)
        //{
        //    // 旋转物体90度
        //    transform.Rotate(0, 0, 90, Space.World);
        //}
        // ¸üÐÂÎïÌåµÄÎ»ÖÃµ½ÐÂµÄÊó±êÎ»ÖÃ¼ÓÉÏÆ«ÒÆÁ¿
        transform.position = GetMouseWorldPos() + offset;

        ReserializeBlockDict();

        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            //TODO: check if blocks are valid, if one is not, then destroy all, else place all and spawn a new object
            if (block != null)
            {
                if (GM.gridSystem.ValidSnap(block)) // ¶ÔÆëÃ¿¸ö·½¿éµ½Grid
                {
                    block.ShowValid();
                }
                else
                {
                    block.ShowInvalid();
                }
            }
        }
    }

    private void InitializeBlockDict()
    {
        foreach(Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            Vector3 blockCoord = GM.gridSystem.GetBlockCoord(block);
            allBlockCoordsDict.Add(block, blockCoord);
        }
    }

    private void ReserializeBlockDict()
    {
        foreach (Transform child in transform)
        {
            Block block = child.GetComponent<Block>();
            allBlockCoordsDict[block] = GM.gridSystem.GetBlockCoord(block);            
        }
        allXY.Clear();
        foreach (KeyValuePair<Block, Vector3> entry in allBlockCoordsDict)
        {
            allXY.Add(entry.Value);
        }
        Vector3 adjustedPosition = new Vector3(transform.position.x, transform.position.y,
            GM.gridSystem.GetHightestZCoord(allXY));

        tempZs.Clear();

        tempZs = GM.gridSystem.GetHightestZCoordList(allXY);
        transform.position = adjustedPosition;
    }

    void OnMouseUp()
    {
        if (placed)
        {
            return;
        }
        placed = true;
        // ¼ÙÉèÕâ¸ö½Å±¾¸½¼ÓÔÚÐÎ×´µÄ¸¸¶ÔÏóÉÏ
        bool validPlacement = true;
        foreach (Transform child in transform)
        {
            if (!validPlacement)
            {
                break;
            }
            Block block = child.GetComponent<Block>();
            //TODO: check if blocks are valid, if one is not, then destroy all, else place all and spawn a new object
            if (block != null)
            {
                //Debug.Log(GM.gridSystem.ValidSnap(block));
                validPlacement = GM.gridSystem.ValidSnap(block); // ¶ÔÆëÃ¿¸ö·½¿éµ½Grid
            }
        }

        if (!validPlacement)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            //respawn old shape
            GM.ShapeGenerator.GeneratePrevShape();
            Debug.Log("regen shape");
            Destroy(gameObject);
            return;
        }
        else
        {
            //TODO: make a beter wireFrame FUncion FK
            //GM.gridSystem.WireFrameTheGrid(GM.currentGrid);
            //spawn in new shape
            GM.ShapeGenerator.GenerateShape();

            //check each placed block to see if it is removeable
            int newZCounter = 0;
            foreach (Transform child in transform)
            {
                if (!validPlacement)
                {
                    break;
                }
                Block block = child.GetComponent<Block>();
                //TODO: check if blocks are valid, if one is not, then destroy all, else place all and spawn a new object
                if (block != null)
                {
                    validPlacement = GM.gridSystem.SnapToGrid(block, tempZs[newZCounter]); // ¶ÔÆëÃ¿¸ö·½¿éµ½Grid
                    newZCounter++;
                }
            }
            //do the remove check
            List<Block> removeBlockList = GM.gridSystem.CheckAndRemoveBlocks();
            //calculate score
            int removedBlocksCount = removeBlockList.Count();
            if (removedBlocksCount > 0)
            {
                // ¸ù¾ÝÒÆ³ýµÄ·½¿éÊýÁ¿¸üÐÂÓÎÏ·Âß¼­£¬ÀýÈç¸üÐÂ·ÖÊý
                GM.AddScore(removedBlocksCount);
            }
            //calculate color combos
            GM.UpdateColorCounts(removeBlockList);

            EventManager.OnDraggablePlaced.Invoke(GM.gridSystem.CheckEntireGridForHighestZ());
            EventManager.CheckGameOver.Invoke();
        }
    }



}
