using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureOverlapArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        measure(-5, 2, -1, 1);
    }

    // Update is called once per frame
    void Update()
    {
    }
    void measure(int minX, int maxX, int minZ, int maxZ)
    {
        Vector3 origin = new Vector3(-1, 3, 0);
        Vector3 direction = new Vector3(-1, -1, 0);
        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        Debug.Log(hits.Length);
        for (int n = 0; n < hits.Length; n++)
        {
            string name = hits[n].collider.gameObject.name; // 衝突した相手オブジェクトの名前を取得
            Debug.Log(name);
        }
        // for (int i = minX; i <= maxX; i++)
        // {
        //     for (int j = minZ; j <= maxZ; j++)
        //     {
        //         Vector3 origin = new Vector3(i, 3, j);
        //         Vector3 direction = new Vector3(i, -1, j);
        //         Ray ray = new Ray(origin, direction);
        //         RaycastHit[] hits;
        //         hits = Physics.RaycastAll(ray);
        //         Debug.Log(hits.Length);
        //         for (int n = 0; n < hits.Length; n++)
        //         {
        //             string name = hits[n].collider.gameObject.name; // 衝突した相手オブジェクトの名前を取得
        //             Debug.Log("x: " + i.ToString() + "z: " + j.ToString() + name);
        //         }
        //     }
        // }
    }
}
