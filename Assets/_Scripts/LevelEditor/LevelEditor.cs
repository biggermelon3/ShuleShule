using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystem))]
public class LevelEditor : Editor
{
    private LevelManager levelManager;

    private void OnEnable()
    {
        levelManager = (LevelManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }

        if (GUILayout.Button("Add Block"))
        {
            AddBlock();
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

    private void AddBlock()
    {
        Vector3 position = new Vector3(Random.Range(0, levelManager.gridSystem.width), Random.Range(0, levelManager.gridSystem.height), Random.Range(0, levelManager.gridSystem.depth));
        Color color = new Color(Random.value, Random.value, Random.value);
        //levelManager.gridSystem.AddBlockAtPosition(position, color);
        Debug.Log("Block added at " + position);
    }
}