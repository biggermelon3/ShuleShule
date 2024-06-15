using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustment : MonoBehaviour
{
    private Camera m_Camera;
    private float positionDifference;
    private int lastHighestZPosition;
    [SerializeField]
    private GridSystem gridSystem; //to get the height, width, depth of grid for camera adjustment

    private GameManager GM;

    [SerializeField]
    private float startingZOffset;
    float absoluteWidth;
    float absoluteHeight;//for calculating camera position
    float absoluteDepth;
    [SerializeField]
    private GameObject baseBlock;//to measure the dimension of a unit

    void Start()
    {
        m_Camera = Camera.main;
        lastHighestZPosition = 9;
        EventManager.OnDraggablePlaced.AddListener(AdjustCameraZPosition);
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GetMiddlePointXY();
        InitializeCameraPosition();
    }

    private void GetMiddlePointXY()
    {
        Debug.Log(GM.gridSystem.height);
        absoluteHeight = baseBlock.GetComponent<MeshFilter>().sharedMesh.bounds.size.y * GM.gridSystem.height;
        absoluteWidth = baseBlock.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * GM.gridSystem.width;
        absoluteDepth = GM.gridSystem.depth;
    }

    private void InitializeCameraPosition()
    {
        Vector3 newCameraPosition = new Vector3(m_Camera.transform.position.x + absoluteWidth / 2 - baseBlock.GetComponent<MeshFilter>().sharedMesh.bounds.size.x / 2,
            m_Camera.transform.position.y + absoluteHeight / 2 - baseBlock.GetComponent<MeshFilter>().sharedMesh.bounds.size.y / 2,
            m_Camera.transform.position.z - absoluteDepth - startingZOffset);
        m_Camera.transform.position = newCameraPosition;
    }

    private void AdjustCameraZPosition(int i)
    {
        Vector3 newCameraPosition = new Vector3(m_Camera.transform.position.x,
            m_Camera.transform.position.y,
            m_Camera.transform.position.z + (i - lastHighestZPosition));
        m_Camera.transform.position = newCameraPosition;
        lastHighestZPosition = i;
    }
}
