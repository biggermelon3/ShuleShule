using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelEditor : Editor
{
    private LevelManager levelManager;
    private GameManager gameManager;
    private Color selectedColor = Color.white;
    private bool isPlacingBlocks = false;
    

    private void OnEnable()
    {
        levelManager = (LevelManager)target;
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.initialColors.Length > 0)
        {
            selectedColor = gameManager.initialColors[0];
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (gameManager != null)
        {
            EditorGUILayout.LabelField("Select Block Color");
            for (int i = 0; i < gameManager.initialColors.Length; i++)
            {
                if (GUILayout.Button("", GUILayout.Width(50), GUILayout.Height(50)))
                {
                    selectedColor = gameManager.initialColors[i];
                }
                Rect lastRect = GUILayoutUtility.GetLastRect();
                EditorGUI.DrawRect(lastRect, gameManager.initialColors[i]);
            }
        }

        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }

        if (GUILayout.Button("Start Placing Blocks"))
        {
            isPlacingBlocks = true;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        if (GUILayout.Button("Stop Placing Blocks"))
        {
            isPlacingBlocks = false;
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }

    private void SaveLevel()
    {
        LevelData levelData = levelManager.GenerateLevelData();
        string json = JsonUtility.ToJson(levelData);
        System.IO.File.WriteAllText(Application.dataPath + "/SavedLevel.json", json);
        Debug.Log("Level saved to " + Application.dataPath + "/SavedLevel.json");
    }

    private void LoadLevel()
    {
        string path = Application.dataPath + "/SavedLevel.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            levelManager.LoadLevelData(levelData);
            Debug.Log("Level loaded from " + path);
        }
        else
        {
            Debug.LogError("Saved level file not found at " + path);
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isPlacingBlocks) return;

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Handles.color = selectedColor;
            Handles.DrawWireCube(hit.point, Vector3.one);

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Vector3 position = hit.point;
                int x = Mathf.RoundToInt(position.x);
                int y = Mathf.RoundToInt(position.y);
                int z = Mathf.RoundToInt(position.z);
                
                GameObject block = Instantiate(levelManager.blockPrefab);

                Block blockComponent = block.GetComponent<Block>(); 
                blockComponent.Initialize(selectedColor);
                
                levelManager.gridSystem.AddBlock(x, y, z, blockComponent);
                
                e.Use();
            }
        }

        SceneView.RepaintAll();
    }
}