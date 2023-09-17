using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapLocation
{
    public int x, z;
    public MapLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}

public class Maze : MonoBehaviour
{
    protected List<MapLocation> directions = new List<MapLocation>
    {
        new MapLocation(1,0),
        new MapLocation(0,1),
        new MapLocation(-1, 0),
        new MapLocation(0, -1)
    };

    [Header("Maze Properties")]
    [SerializeField] protected int width;
    [SerializeField] protected int depth;
    public byte[,] map;
    public int scale = 6;

    [Header("Maze pieces")]
    public List<GameObject> straights;
    public List<GameObject> crossRoads;
    public List<GameObject> deadEnds;
    public List<GameObject> corners;
    public List<GameObject> tPieces;
    //public List<GameObject> pipes;
    public GameObject wallPiece, floorPiece, ceilingPiece;
    
    [Header("")]
    public GameObject player;


    public enum PieceType { 
    
    }

    void Start()
    {
        InitializeMap();
        Generate();
        AddRooms(3, 4, 6);
        DrawMap();
        PlacePlayer();
    }

    private void AddRooms(int count, int minSize, int maxSize)
    {
        for (int i = 0; i < count; i++)
        {
            int startX = UnityEngine.Random.Range(3, width -3);
            int startZ = UnityEngine.Random.Range(3, depth - 3);
            int roomWidth = UnityEngine.Random.Range(minSize, maxSize);
            int roomDepth = UnityEngine.Random.Range(minSize, maxSize);
            
            for (int x = startX; x < width - 3 && x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < depth - 3 && z< startZ + roomDepth; z++)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    public virtual void PlacePlayer()
    {

        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < depth; x++)
            {
                if (map[x, z] == 0) {
                    player.transform.position = new Vector3(x * scale, 0 , z * scale);
                    return;
                }
            }
        }
    }

    private void InitializeMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < width; z++){
            for (int x = 0; x < depth; x++)
            {
                map[x, z] = 1;
            }
        }
    }

    public virtual void Generate()
    {
        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < depth; x++)
            {                
                if (UnityEngine.Random.Range(0, 100) < 50)                    
                    map[x, z] = 0;
            }
        }
    }
     
    //Instantiate map 
    private void DrawMap()
    {
        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < depth; x++)
            {
                //Maze Cube for wall
                if (map[x, z] == 1)
                {
                    //Vector3 pos = new Vector3(x * scale, 0 * scale, z * scale);
                    //GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //wall.transform.localScale = new Vector3(scale, scale, scale);
                    //wall.transform.position = pos;
                    //wall.transform.SetParent(gameObject.transform.parent);
                }
                //Dead-End pieces
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 }))//Horizontal end -|
                {
                    GameObject go = Instantiate(deadEnds[UnityEngine.Random.Range(0, deadEnds.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 }))//Horizontal end |-
                {
                    GameObject go = Instantiate(deadEnds[UnityEngine.Random.Range(0, deadEnds.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, 0, 0);

                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 }))//Vertical end T
                {
                    GameObject go = Instantiate(deadEnds[UnityEngine.Random.Range(0, deadEnds.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, 90, 0);

                }
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 }))//Vertical end upside down T
                {
                    GameObject go = Instantiate(deadEnds[UnityEngine.Random.Range(0, deadEnds.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, -90, 0);

                }

                //Vertical Pieces FIXED
                else if (Search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 0, 5 }))
                {
                    Vector3 pos = new Vector3(x * scale, 0 * scale, z * scale);
                    int randomIndex = UnityEngine.Random.Range(0, straights.Count);
                    GameObject go = Instantiate(straights[randomIndex], pos, Quaternion.identity);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.Rotate(0, 90, 0);

                }

                //Horizontal Pieces FIXED
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 }))
                {
                    int randomIndex = UnityEngine.Random.Range(0, straights.Count);
                    Vector3 pos = new Vector3(x * scale, 0 * scale, z * scale);
                    GameObject go = Instantiate(straights[randomIndex], pos, Quaternion.identity);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.Rotate(0, 0, 0);
                }

                //Crossroad pieces
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 }))
                {
                    GameObject go = Instantiate(crossRoads[UnityEngine.Random.Range(0, crossRoads.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                //Corner pieces
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 }))//Upper Right corner
                {
                    int rndPiece = UnityEngine.Random.Range(0, corners.Count);
                    GameObject go = Instantiate(corners[rndPiece]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);


                    go.transform.Rotate(0, 180, 0);

                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 }))//Upper Left corner
                {
                    int rndPiece = UnityEngine.Random.Range(0, corners.Count);
                    GameObject go = Instantiate(corners[rndPiece]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);


                    go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 }))//Lower Left corner
                {
                    int rndPiece = UnityEngine.Random.Range(0, corners.Count);
                    GameObject go = Instantiate(corners[rndPiece]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    if (rndPiece == 0)
                        go.transform.Rotate(0, 0, 0);

                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 5, 0, 1, 5, 1, 5 }))//Lower Right corner
                {
                    int rndPiece = UnityEngine.Random.Range(0, corners.Count);
                    GameObject go = Instantiate(corners[rndPiece]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);

                    go.transform.Rotate(0, 270, 0);
                }
                //T-Junction pieces
                else if (Search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 }))//T-Junc Upside down
                {
                    GameObject go = Instantiate(tPieces[UnityEngine.Random.Range(0, tPieces.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, -90, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 }))//T-Junc T
                {
                    GameObject go = Instantiate(tPieces[UnityEngine.Random.Range(0, tPieces.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, 90, 0);
                }
                else if (Search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 1, 0, 5 }))//T-Junc -|
                {
                    GameObject go = Instantiate(tPieces[UnityEngine.Random.Range(0, tPieces.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                    go.transform.Rotate(0, 180, 0);
                }
                else if (Search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 0, 1 }))//T-Junc |-
                {
                    GameObject go = Instantiate(tPieces[UnityEngine.Random.Range(0, tPieces.Count)]);
                    go.transform.SetParent(gameObject.transform);
                    go.transform.position = new Vector3(x * scale, 0, z * scale);
                }
                
                //Room
                else if (map[x,z] == 0 && 
                    (CountSquareNeighbours(x, z) > 1 && CountDiagonalNeighbours(x, z) >= 1 ||
                    CountSquareNeighbours(x, z) >= 1 && CountDiagonalNeighbours(x, z) >= 1))
                {
                    GameObject floor = Instantiate(floorPiece);
                    GameObject ceiling = Instantiate(ceilingPiece);
                    floor.transform.position = new Vector3(x * scale, 0, z * scale);
                    ceiling.transform.position = new Vector3(x * scale, 0, z * scale);



                    LocateWalls(x, z);
                    if (top) {

                        GameObject wallTop = Instantiate(wallPiece);
                        wallTop.transform.position = new Vector3(x * scale, 0, z * scale);
                        wallTop.transform.Rotate(0, 90, 0);
                    }

                    if (bottom)
                    {
                        GameObject wallBottom = Instantiate(wallPiece);
                        wallBottom.transform.position = new Vector3(x * scale, 0, z * scale);
                        wallBottom.transform.Rotate(0, -90, 0);
                    }

                    if (left)
                    {
                        GameObject wallLeft = Instantiate(wallPiece);
                        wallLeft.transform.position = new Vector3(x * scale, 0, z * scale);
                        wallLeft.transform.Rotate(0, 0, 0);

                    }

                    if (right)
                    {
                        GameObject wallRight = Instantiate(wallPiece);
                        wallRight.transform.position = new Vector3(x * scale, 0, z * scale);
                        wallRight.transform.Rotate(0, 180, 0);
                    }
                }

                else
                {
                    Vector3 pos = new Vector3(x * scale, 0 * scale, z * scale);
                    GameObject block = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    block.transform.localScale = new Vector3(scale, scale, scale);
                    block.transform.position = pos;
                    //block.transform.SetParent(gameObject.transform.parent);
                }

            }
        }
    }

    bool top, bottom, left, right;

    public void LocateWalls(int x, int z) {
        top = false;
        bottom = false;
        right = false;
        left = false;

        if (x <= 0 || x >= width-1 || z <=0 || z >= depth-1) return;
        if (map[x, z + 1] == 1) top = true;
        if (map[x, z - 1] == 1) bottom = true;
        if (map[x + 1, z] == 1) right = true;
        if (map[x - 1, z] == 1) left = true;
        


    }

    bool Search2D(int c, int r, int [] pattern) {
        int count = 0, pos = 0;
        for (int z = 1; z > -2; z--)
        {
            for (int x = -1; x < 2; x++)
            {
                if (pattern[pos] == map[c + x, r + z] || pattern[pos] == 5)
                {
                    count++;
                }
                    pos++;
            }
        }
        return(count == 9);
    }

    public int CountSquareNeighbours(int x, int z) {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z] == 0) count++;
        if(map[x + 1, z] == 0) count++;
        if(map[x, z + 1] == 0) count++;
        if(map[x, z - 1] == 0) count++;       
        return count;
    }

    public int CountDiagonalNeighbours(int x, int z) {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, z -1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z - 1] == 0) count++;
        return count;
    }

    public int CountAllNeighbours(int x, int z) {        
        return CountSquareNeighbours(x, z) + CountDiagonalNeighbours(x, z);
    }
}
