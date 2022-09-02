using System;
using System.Collections.Generic;
using UnityEngine;
public class Root
{
    public List<Map> maps;
    public List<Destination> destinations;
}

[Serializable]
public class Map
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public List<Vector3> meshVertices;
    public List<int> meshTriangles;
}
[Serializable]
public class Destination
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string[] textData;
}

public class Marker
{
    public int id;
    public Vector3 position;

    public Marker(int n, Vector3 pos)
    {
        id = n;
        position = pos;
    }
}

public static class CommonVariables
{
    public static List<GameObject> destinationList = new List<GameObject>();
    public static int currntFloor = 0;
}