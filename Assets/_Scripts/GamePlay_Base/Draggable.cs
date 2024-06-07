using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private GameManager GM;
    private bool placed = false;

    void Start()
    {
        mainCamera = Camera.main; // »ñÈ¡Ö÷Ïà»ú
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void OnMouseDown()
    {
        if (placed)
        {
            return;
        }
        

        // ¼ÆËãÎïÌåÓëÊó±êµã»÷µãµÄÆ«ÒÆÁ¿
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (placed)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && !placed)
        {
            // 旋转物体90度
            transform.Rotate(0, 0, 90, Space.World);
        }
        // ¸üÐÂÎïÌåµÄÎ»ÖÃµ½ÐÂµÄÊó±êÎ»ÖÃ¼ÓÉÏÆ«ÒÆÁ¿
        transform.position = GetMouseWorldPos() + offset;
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
            Destroy(gameObject);
            return;
        }
        else
        {
            //TODO: spawn in new shape
            GM.ShapeGenerator.GenerateShape();
            //TODO: check each placed block to see if it is removeable
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
                    validPlacement = GM.gridSystem.SnapToGrid(block); // ¶ÔÆëÃ¿¸ö·½¿éµ½Grid
                }
            }
            // ¿ÉÑ¡£ºÔÚÕâÀïµ÷ÓÃ¼ì²éºÍÏû³ýÏàÍ¬ÑÕÉ«Á¬Ðø·½¿éµÄÂß¼­
            int removedBlocksCount = GM.gridSystem.CheckAndRemoveBlocks();
            if (removedBlocksCount > 0)
            {
                // ¸ù¾ÝÒÆ³ýµÄ·½¿éÊýÁ¿¸üÐÂÓÎÏ·Âß¼­£¬ÀýÈç¸üÐÂ·ÖÊý
                GM.AddScore(removedBlocksCount);
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        // ½«Êó±êÔÚÆÁÄ»ÉÏµÄÎ»ÖÃ×ª»»ÎªÔÚÊÀ½çÖÐµÄÎ»ÖÃ
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(gameObject.transform.position).z; // È·±£ÎïÌåÑØ×ÅZÖá²»ÒÆ¶¯
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}