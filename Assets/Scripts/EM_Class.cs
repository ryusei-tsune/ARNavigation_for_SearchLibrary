using System;
using System.Collections.Generic;
using UnityEngine;
public class Root {
    public List<Map> maps;
    public List<Destination> destinations;
}

[Serializable]
public class Map {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public List<Vector3> meshVertices;
    public List<int> meshTriangles;
}
[Serializable]
public class Destination {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string[] textData;
}