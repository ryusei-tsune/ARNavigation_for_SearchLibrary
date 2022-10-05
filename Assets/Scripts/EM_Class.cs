using System;
using System.Collections.Generic;
using UnityEngine;
// 環境マップの根幹
public class Root
{
    public List<Map> maps;
    public List<Destination> destinations;
    public List<MovingPoint> movingPoints;
}
// 環境マップの歩行可能領域に関する情報を保持
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
// 環境マップの目的地(本棚)に関する情報を保持
public class Destination
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string[] textData;
}
[Serializable]
// エレベータや階段等の階層移動地点に関する情報を保持
public class MovingPoint{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}
// マーカIDと座標をセットで管理
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
// 現在の階や目的地等，複数のクラスで利用する変数を保持
public static class CommonVariables
{
    public static List<GameObject> destinationList = new List<GameObject>();
    public static List<GameObject> movingPointList = new List<GameObject>();
    public static int currntFloor = 0;
}