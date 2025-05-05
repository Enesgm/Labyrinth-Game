using System;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public int width = 15;
    public int height = 15;
    public float cellSize = 2.0f;

    public GameObject floorPrefab;
    public GameObject wallPrefab;

    private bool[,] visitedCells;
    private System.Collections.Generic.List<Vector2Int> walls = new System.Collections.Generic.List<Vector2Int>();

    void Start()
    {
        //Oyun başladığında labirenti oluştur
        GenerateMaze();
    }

    //Labirenti oluştur
    private void GenerateMaze()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Ziyaret edilmiş hücreleri takip etmek için dizi oluştur
        visitedCells = new bool[width, height];

        // Önce zemini oluştur
        CreateFloor();

        // Tüm duvarları oluştur
        CreateAllWalls();

        // Labirent algoritmasını başlat (sol alt köşeden)
        Vector2Int startPos = new Vector2Int(0, 0);
        GeneratePath(startPos);

        Debug.Log("Labirent oluşturuldu!");
    }

    //Zemini oluştur
    void CreateFloor()
    {
        //Konum belirle ve floor adında gameobject oluştur
        Vector3 position = new Vector3(width * cellSize / 2, -0.5f, height * cellSize / 2);
        GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);

        //Zemin boyutunu ve objenin çocuğu olarak ayarla
        floor.transform.localScale = new Vector3(width * cellSize, 1, height * cellSize);
        floor.transform.parent = transform;
    }

    //Duvarları oluştur
    void CreateAllWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                wall.transform.parent = transform;
            }
        }
    }

    void GeneratePath(Vector2Int currentCell)
    {
        visitedCells[currentCell.x, currentCell.y] = true;

        System.Collections.Generic.List<Vector2Int> neighbors = GetUnvisitedNeighbors(currentCell);
        ShuffleList(neighbors);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (!visitedCells[neighbor.x, neighbor.y])
            {
                RemoveWall(currentCell, neighbor);

                GeneratePath(neighbor);
            }
        }
    }

    System.Collections.Generic.List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        System.Collections.Generic.List<Vector2Int> neighbors = new System.Collections.Generic.List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = cell + dir;

            if (neighbor.x >= 0 && neighbor.x < width &&
               neighbor.y >= 0 && neighbor.y < height)
            {
                if (!visitedCells[neighbor.x, neighbor.y])
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }

    void RemoveWall(Vector2Int cell1, Vector2Int cell2)
    {
        // İki hücre arasındaki duvarın pozisyonunu hesapla
        Vector2Int wallPosition = (cell1 + cell2) / 2;

        // Hücre koordinatlarını sahne koordinatlarına çevir
        Vector3 position = new Vector3(wallPosition.x * cellSize, 0, wallPosition.y * cellSize);

        // Bu pozisyondaki duvarı bul ve yok et
        foreach (Transform child in transform)
        {
            if (Vector3.Distance(child.position, position) < 0.1f)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    void ShuffleList<T>(System.Collections.Generic.List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
