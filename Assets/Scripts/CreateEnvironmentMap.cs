using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NavMeshExtension;
public class CreateEnvironmentMap : MonoBehaviour
{
    private GameObject mesh;
    private NavMeshObject navMesh;
    private NavMeshManager navManager;
    private GameObject newNavMesh;
    [SerializeField] GameObject mapInstantiate;
    [SerializeField] Material mapMaterial;
    [SerializeField] GameObject particle;

    List<GameObject> particles = new List<GameObject>();
    [SerializeField] Vector3[] allPoints;
    private static Vector3 yValue;
    private static bool placing = false;
    private static bool landmark = false;

    private Vector3 touchPosition;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    [SerializeField] GameObject elevator;

    [SerializeField] GameObject landMark; // 目的地となる物体
    private List<GameObject> landMarkLists = new List<GameObject>(); // 目的地の情報を全て保持
    private int landMarkSelect = -1;
    // [SerializeField] GameObject upStair; // 階層移動地点
    // private List<GameObject> upStairs = new List<GameObject>(); // 目的地の情報を全て保持
    // private int upStairSelect = -1;
    // [SerializeField] GameObject downStair; // 階層移動地点
    // private List<GameObject> downStairs = new List<GameObject>(); // 目的地の情報を全て保持
    // private int downStairSelect = -1;

    [SerializeField] Text statusText;

    private void Start()
    {
        statusText.enabled = false;
        raycastManager = GetComponent<ARRaycastManager>();
    }
    private void Update()
    {
        /*　Began:画面に指が触れたとき
        Moved:画面上で指が動いたとき
        Stationary:指が画面に触れているが動いてはいないとき，
        Ended:画面から指が離れたとき
        Canceled:システムがタッチの追跡をキャンセルしました*/
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
            else
            {
                Touch touch = Input.GetTouch(0);
                if (raycastManager.Raycast(touch.position, hitResults, TrackableType.Planes))
                {
                    touchPosition = hitResults[0].pose.position;
                    if (yValue == Vector3.zero)
                    {
                        yValue = touchPosition;
                    }
                    else
                    {
                        touchPosition.y = yValue.y;
                    }
                    if (placing)
                    {
                        // navMeshに登録されている点を配列で取得
                        allPoints = ConvertAllPoints();
                        int dragIndex = FindClosest(touch);
                        particles.Add(Instantiate(particle));
                        particles[particles.Count - 1].transform.position = touchPosition;

                        if (navMesh.current.Contains(dragIndex))
                        {
                            navMesh.AddPoint(navMesh.transform.TransformPoint(navMesh.list[dragIndex]));
                        }
                        else if (dragIndex >= 0)
                        {
                            navMesh.AddPoint(dragIndex);
                        }
                        else
                        {
                            navMesh.AddPoint(touchPosition);
                        }
                        navMesh.CreateMesh();
                    }
                    else
                    {
                        if (landmark)
                        {
                            if (landMarkSelect != -1)
                            {
                                // 目的地の物体を移動
                                landMarkLists[landMarkSelect].transform.position = touchPosition;
                            }
                            else
                            {
                                // 新規で目的地の物体を作成
                                landMarkLists.Add(Instantiate(landMark, touchPosition, Quaternion.identity));
                                landMarkSelect = landMarkLists.Count - 1;
                            }
                        }//  else {
                        //     elevatorObject.transform.position = touchPosition;
                        // }
                    }
                }
            }
        }
    }

    private Vector3[] ConvertAllPoints()
    {
        int count = navMesh.list.Count;
        List<Vector3> all = new List<Vector3>();
        for (int i = 0; i < count; i++)
            all.Add(navMesh.transform.TransformPoint(navMesh.list[i]));

        return all.ToArray();
    }

    private int FindClosest(Touch touch)
    {
        List<int> closest = new List<int>();
        int nearPoint = -1;
        for (int i = 0; i < allPoints.Length; i++)
        {
            Vector2 screenPoint = Camera.current.WorldToScreenPoint(allPoints[i]);
            if (Vector2.Distance(screenPoint, touch.position) < 100)
            {
                closest.Add(i);
            }
        }

        if (closest.Count == 0)
        {
            return nearPoint;
        }
        else if (closest.Count == 1)
        {
            return closest[0];
        }
        else
        {
            Vector3 camPos = Camera.current.transform.position;
            float nearDist = float.MaxValue;
            for (int i = 0; i < closest.Count; i++)
            {
                float dist = Vector3.Distance(allPoints[closest[i]], camPos);
                if (dist < nearDist)
                {
                    nearDist = dist;
                    nearPoint = closest[i];
                }
            }
        }
        closest.Clear();
        return nearPoint;
    }

    public void CreateMesh()
    {
        mesh = new GameObject("mesh");
        mesh.AddComponent<NavMeshObject>();
        mesh.AddComponent<NavMeshManager>();
        statusText.enabled = true;
        particles.Clear();
        navManager = (GameObject.FindGameObjectsWithTag("MapGameObject") == null) ? Instantiate(mapInstantiate).GetComponent<NavMeshManager>() : GameObject.FindGameObjectWithTag("MapGameObject").GetComponent<NavMeshManager>();
        newNavMesh = new GameObject("New NavMesh");

        newNavMesh.transform.parent = navManager.transform;
        newNavMesh.AddComponent<NavMeshObject>();
        newNavMesh.AddComponent<NavMeshModifier>();
        newNavMesh.AddComponent<MeshFilter>();
        newNavMesh.AddComponent<MeshRenderer>();
        newNavMesh.tag = "mapNavMesh";

        navMesh = newNavMesh.GetComponent<NavMeshObject>();
        MeshRenderer mRenderer = newNavMesh.GetComponent<MeshRenderer>();
        mRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mRenderer.receiveShadows = false;
        if (navManager.meshMaterial)
        {
            navManager.meshMaterial.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
            mRenderer.sharedMaterial = navManager.meshMaterial;
            statusText.text = "Create";
        }
        else
        {
            mRenderer.enabled = false;
        }
    }
}
