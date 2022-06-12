using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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
    private List<int> selected = new List<int>(); // 必要ないかも
    private static Vector3 yValue= Vector3.zero;
    private static bool placing = false;
    private static bool landmark = false;

    private Vector3 touchPosition;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

    [SerializeField] GameObject elevator;

    [SerializeField] GameObject destination; // 目的地となる物体
    private List<GameObject> destinationLists = new List<GameObject>(); // 目的地の情報を全て保持
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
        raycastManager = GameObject.FindGameObjectWithTag("ARSessionOrigin").GetComponent<ARRaycastManager>();
        CreateMesh();
    }
    private void Update()
    {
        /*　TouchPhase
        Began:画面に指が触れたとき
        Moved:画面上で指が動いたとき
        Stationary:指が画面に触れているが動いてはいないとき，
        Ended:画面から指が離れたとき
        Canceled:システムがタッチの追跡をキャンセルしました
        */
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

                    // navMeshに登録されている点を配列で取得
                    allPoints = ConvertAllPoints();

                    int dragIndex = FindClosest(touch);

                    if (placing)
                    {
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
                        statusText.text = landmark.ToString();
                        if (landmark)
                        {
                            if (landMarkSelect != -1)
                            {
                                statusText.text = "bbbb";
                                // 目的地の物体を移動
                                destinationLists[landMarkSelect].transform.position = touchPosition;
                            }
                            else
                            {
                                statusText.text = "aaaa";
                                // 新規で目的地の物体を作成
                                destinationLists.Add(Instantiate(destination, touchPosition, Quaternion.identity));
                                landMarkSelect = destinationLists.Count - 1;
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
        for (int i = 0; i < count; i++) all.Add(navMesh.transform.TransformPoint(navMesh.list[i]));
        
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
        navMesh =  mesh.AddComponent<NavMeshObject>();
        navManager = mesh.AddComponent<NavMeshManager>();
        statusText.enabled = true;
        particles.Clear();
        if (GameObject.FindGameObjectsWithTag("MapGameObject") == null)
        {
            navManager = Instantiate(mapInstantiate).GetComponent<NavMeshManager>();
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag("MapGameObject"));
            navManager = Instantiate(mapInstantiate).GetComponent<NavMeshManager>();
        }
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
    public void MapButton()
    {
        landmark = false;
        landMarkSelect = -1;
        // PTタグを付与した選択地点のパーティクルを検出
        if (!placing)
        {
            GameObject newObj = newNavMesh.GetComponent<NavMeshObject>().CreateSubMesh();
            placing = true;
            statusText.text = "map";
        }
        else
        {
            ConfirmMap();
        }
    }

    public void LandMarkButton()
    {
        placing = false;
        ConfirmMap();
        // if (!landmark)
        // {
        //     landmark = true;
        // }
        // else
        // {
        //     landmark = false;
        // }
        landmark = true;
        statusText.text = "landmark";
    }
    // 指定した歩行可能領域の確定
    private void ConfirmMap()
    {
        GameObject[] particleList = GameObject.FindGameObjectsWithTag("PT");
        foreach (var item in particleList)
        {
            Destroy(item); //  item.SetActive(false);
        }
        CheckCombine();

        // //get all mesh filters
        MeshFilter[] meshFilters = navMesh.GetComponentsInChildren<MeshFilter>();
        //let the navmesh combine them   
        navMesh.Combine();
        for (int i = 1; i < meshFilters.Length; i++)
        {
            Destroy(meshFilters[i].gameObject);
        }
    }

    private void CheckCombine()
    {

        //get count of all submeshes
        int subPointsCount = navMesh.subPoints.Count;
        if (navMesh.subPoints.Count == 0) return;
        NavMeshObject.SubPoints lastPoints = navMesh.subPoints[subPointsCount - 1];

        if (lastPoints.list.Count <= 2)
        {
            selected.Clear();
            for (int i = 0; i < lastPoints.list.Count; i++)
            {
                selected.Add(lastPoints.list[i]);
            }

            DeleteSelected();
        }
    }

    //delete previously selected vertices and rebuild mesh
    private void DeleteSelected(bool auto = false)
    {
        //get mesh references
        MeshFilter filter = navMesh.GetComponent<MeshFilter>();
        List<Vector3> vertices = null;
        if (filter.sharedMesh != null)
            vertices = new List<Vector3>(filter.sharedMesh.vertices);
        //filter selected list for unique entries
        selected = selected.Distinct().ToList();
        selected = selected.OrderByDescending(x => x).ToList();

        //loop over selected vertex indices
        for (int i = 0; i < selected.Count; i++)
        {
            //remove index from mesh vertices
            int index = selected[i];
            if (vertices != null)
            {
                if (vertices.Contains(allPoints[index])) vertices.Remove(allPoints[index]);
                else try { vertices.RemoveAt(index); } catch { };
            }
            navMesh.list.RemoveAt(index);

            //loop over submeshes and remove it there too
            for (int j = 0; j < navMesh.subPoints.Count; j++)
            {
                navMesh.subPoints[j].list.Remove(index);
                //decrease higher entries, as the array is smaller now
                for (int k = 0; k < navMesh.subPoints[j].list.Count; k++)
                {
                    if (navMesh.subPoints[j].list[k] >= index)
                        navMesh.subPoints[j].list[k] -= 1;
                }
            }
        }

        //clear selection
        selected.Clear();

        //loop over submeshes to remove obsolete indices,
        //e.g. if a submesh has only 2 vertices after removal
        for (int i = navMesh.subPoints.Count - 1; i >= 0; i--)
        {
            //check for vertex count
            if (navMesh.subPoints[i].list.Count <= 2)
            {
                //construct a combined list with all indices
                List<int> allIndices = new List<int>();
                for (int j = 0; j < navMesh.subPoints.Count; j++)
                    allIndices.AddRange(navMesh.subPoints[j].list);

                //check whether an index occurs more than once
                List<int> duplicates = allIndices.GroupBy(x => x)
                                       .Where(x => x.Count() > 1)
                                       .Select(x => x.Key)
                                       .ToList();

                //if an index in this submesh is not being used in other
                //submeshes anymore, this means that we can remove it too
                for (int j = 0; j < navMesh.subPoints[i].list.Count; j++)
                    if (!duplicates.Contains(navMesh.subPoints[i].list[j]))
                        selected.Add(navMesh.subPoints[i].list[j]);

                //delete this submesh entry
                navMesh.subPoints.RemoveAt(i);
            }
        }

        //recalculate triangles for complete mesh
        if (filter.sharedMesh == null) return;
        List<int> triangles = new List<int>();
        for (int i = 0; i < navMesh.subPoints.Count; i++)
            triangles.AddRange(navMesh.RecalculateTriangles(navMesh.subPoints[i].list));

        //assign triangles and update vertices
        filter.sharedMesh.triangles = triangles.ToArray();
        navMesh.list = vertices;
        navMesh.UpdateMesh(ConvertAllPoints());

        //recursively delete the remaining obsolete indices
        //which were found by looking through all submeshes
        if (selected.Count > 0)
        {
            DeleteSelected();
            return;
        }

        //deletion done - optimize mesh
        OptimizeMesh(filter.sharedMesh);
    }
    //rebuild mesh properties
    private void OptimizeMesh(Mesh Opmesh)
    {
        Opmesh.RecalculateNormals();
        Opmesh.RecalculateBounds();
    }
}
