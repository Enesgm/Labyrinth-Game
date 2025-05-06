using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefab Referansları")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject columnPrefab;
    public GameObject startPrefab;
    public GameObject endPrefab;

    [Header("Labirent Boyutu")]
    public int width = 15;
    public int height = 15;

    //Hücre tipi için enum
    private enum CellType
    {
        Empty,
        Wall,
        Start,
        End
    }

    //Labirent grid'i
    private CellType[,] maze;

    //Hücreleri tutmak için
    private GameObject[,] mazeObjects;

    //Mevcut labirent adı
    public string currentMazeName = "Maze1";

    void Start()
    {
        GenerateMaze();
    }

    //Labirent oluşturma fonksiyonu
    void GenerateMaze()
    {
        //Grid'leri başlat
        maze = new CellType[width, height];
        mazeObjects = new GameObject[width, height];

        //Başlangıçta her yeri duvar yap
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z] = CellType.Wall;
            }
        }

        //Derinlik öncelikli arama ile labirent oluşturma
        int startX = 1;
        int startZ = 1;
        maze[startX, startZ] = CellType.Start;

        GeneratePath(startX, startZ);

        //Bitiş noktasını koy (Örneğin en uzak nokta)
        int endX = width - 2;
        int endZ = width - 2;
        maze[endX, endZ] = CellType.End;

        //Labirenti inşa et
        BuildMaze();
    }

    //Yol oluşturma (Derinlik öncelikli arama)
    void GeneratePath(int x, int z)
    {
        //4 yön: Yukarı, Sağ, Aşağı, Sol
        int[] dx = { 0, 1, 0, -1 };
        int[] dz = { 1, 0, -1, 0 };

        //Yönleri karıştır
        List<int> directions = new List<int> { 0, 1, 2, 3 };
        Shuffle(directions);

        //Her yönü dene
        foreach (int dir in directions)
        {
            int nx = x + dx[dir] * 2;
            int nz = z + dz[dir] * 2;

            //Sınırlar içinde mi ve duvar mı?
            if (nx >= 0 && nx < width && nz >= 0 && nz < height && maze[nx, nz] == CellType.Wall)
            {
                //Duvarları yık (Empty yap)
                maze[x + dx[dir], z + dz[dir]] = CellType.Empty;
                maze[nx, nz] = CellType.Empty;

                //Rekürsif olarak devam et
                GeneratePath(nx, nz);
            }
        }
    }

    //Listeyi karıştırma fonksiyonu
    void Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    //Labirenti fiziksel olarak inşa et
    void BuildMaze()
    {
        //Ana labirent objesini oluştur
        GameObject mazeParent = new GameObject("Maze_" + currentMazeName);

        //Zemin ve duvarları yerleştir
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x, 0, z);

                //Tüm hücrelere zemin ekle
                GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);
                floor.transform.parent = mazeParent.transform;

                //Hücre tipine göre obje ekle
                switch (maze[x, z])
                {
                    case CellType.Wall:
                        //Duvar ekle
                        GameObject wall = Instantiate(wallPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        wall.transform.parent = mazeParent.transform;
                        break;

                    case CellType.Start:
                        //Başlangıç noktası
                        GameObject start = Instantiate(startPrefab, position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                        start.transform.parent = mazeParent.transform;
                        break;

                    case CellType.End:
                        //Bitiş noktası
                        GameObject end = Instantiate(endPrefab, position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                        end.transform.parent = mazeParent.transform;
                        break;
                }
            }
        }

        //Kolonları ekle (köşeler için)
        AddColumns(mazeParent.transform);

        //Labirenti kaydet
        SaveMaze();
    }
}
