using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustment : MonoBehaviour
{
    private Camera m_Camera;
    private float positionDifference;
    private int lastHighestZPosition;
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        lastHighestZPosition = 9;
        EventManager.OnDraggablePlaced.AddListener(AdjustCameraZPosition);
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
