/**
* Game of Life Implementation
* @author Sinead Urisohn
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private Tilemap tileMap;
    private WorldData worldData;
    private Color painterCol;
    const string folderName = "SavedWorlds";
    const string fileExtension = ".dat";

    private void Start()
    {
        worldData = new WorldData();
        painterCol = GameOfLife.OnColors[GameOfLife.colorPos];
    }

    void FixedUpdate ()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousPos.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            // get the collision point of the ray with the z = 0 plane
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / 1.0f * ray.direction.z);
            Vector3Int position = tileMap.WorldToCell(mousPos);
            SetCellColour(position, painterCol);
            //Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {0}]", Input.mousePosition.x, Input.mousePosition.y));
            //Debug.Log(string.Format("Co-ords of CELL is [X: {0} Y: {0}]", position.x, position.y));

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearWorld();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (painterCol == GameOfLife.OffColors[GameOfLife.colorPos])
                painterCol = GameOfLife.OnColors[GameOfLife.colorPos];
            else
                painterCol = GameOfLife.OffColors[GameOfLife.colorPos];
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            List<SerializableVector3Int> results = new List<SerializableVector3Int>();
            foreach (var position in tileMap.cellBounds.allPositionsWithin)
            {
                if(tileMap.GetColor(position) == Color.black)
                {
                    results.Add(position);
                }
            }
            worldData.aliveCellPositions = results.ToArray();
            string folderPath = Path.Combine(Application.persistentDataPath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            string dataPath = Path.Combine(folderPath, worldData.id + fileExtension);
            SaveWorld(worldData, dataPath);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ClearWorld();
            string[] filePaths = GetFilePaths();
            string folderPath = Path.Combine(Application.persistentDataPath, folderName);

            worldData = LoadWorld(Path.Combine(folderPath, worldData.id + fileExtension));

            DrawWorld();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameOfLife.startSim = true;
        }


    }

    private void DrawWorld()
    {
        foreach (var position in worldData.aliveCellPositions)
        {
            SetCellColour(position, GameOfLife.OnColors[GameOfLife.colorPos]);
        }
    }

    private void SetCellColour(Vector3Int pos, Color col)
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, col);
    }

    private void SaveWorld(WorldData worldData, string dataPath)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, worldData);
        }
        Debug.Log("Saved World to " + dataPath);
    }

    static WorldData LoadWorld(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            Debug.Log("Loaded World from " + path);
            return (WorldData)binaryFormatter.Deserialize(fileStream);
        }
        
    }

    static string[] GetFilePaths()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        return Directory.GetFiles(folderPath, fileExtension);
    }

    private void ClearWorld()
    {
        foreach (var position in tileMap.cellBounds.allPositionsWithin)
        {
            tileMap.SetColor(position, GameOfLife.OffColors[GameOfLife.colorPos]);
        }
    }

    private void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        GameOfLife.startSim = false;
    }
}
