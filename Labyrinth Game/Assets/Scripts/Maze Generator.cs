using System;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public int width = 15;
    public int height = 15;
    public float cellSize = 2.0f;

    public GameObject floorPrefab;

    void Start()
    {
        //Oyun başladığında labirenti oluştur
        GenerateMaze();
    }

    //Labirenti oluştur
    private void GenerateMaze()
    {
        CreateFloor();
    }

    //Zemini oluştur
    void CreateFloor()
    {
        Vector3 position = new Vector3(width * cellSize / 2, -0.5f, height * cellSize / 2);
        GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);

        floor.transform.localScale = new Vector3(width * cellSize, 0, height * cellSize);
        floor.transform.parent = transform;
    }
}
