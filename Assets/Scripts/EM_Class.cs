using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class Root {
    public int a;
    public List<Map> maps;
    public List<LandMark> landMarks;
}

[System.Serializable]
public class Map {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public List<Vector3> meshVertices;
    public List<int> meshTriangles;
}
[System.Serializable]
public class LandMark {
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string[] textData;
}