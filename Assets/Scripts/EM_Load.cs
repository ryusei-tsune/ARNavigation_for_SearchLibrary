using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
public class EM_Load : MonoBehaviour
{
    [SerializeField] ARSession m_ARSession;
    [SerializeField] Text statusText;
    [SerializeField] GameObject mapInstantiate;
    [SerializeField] Material mapMaterial;
    [SerializeField] GameObject destination;
    private string featurePath;
    private string objectPath;
    private GameObject ARMap;
    private List<GameObject> mapList = new List<GameObject>();
    private List<GameObject> destinationList = new List<GameObject>();
    public void LoadButton()
    {
        featurePath = Application.persistentDataPath + "/" + "test-Feature.ARMap";
        objectPath = Application.persistentDataPath + "/" + "test-Object.json";
        if (File.Exists(featurePath)) File.Delete(featurePath);
        if (File.Exists(objectPath)) File.Delete(objectPath);
#if UNITY_IOS
        StartCoroutine(LoadFeature());
#endif
        ARMap = Instantiate(mapInstantiate);
        LoadObject();
    }

    private void LoadObject()
    {
        string json = "";
        try
        {
            using (StreamReader sr = new StreamReader(objectPath))
            {
                json = sr.ReadLine();
            }
        }
        catch (Exception e)
        {
            statusText.text = e.ToString();
        }
        /*{"maps":
            [
                {
                    "position":{"x":1.0,"y":2.0,"z":3.0},
                    "rotation":{"x":0.0,"y":0.0,"z":0.0},
                    "scale":{"x":0.0,"y":0.0,"z":0.0},
                    "meshVertices":[],
                    "meshTriangles":[]
                }
            ],
            "destinations":
            [

            ]
        }*/
        Root root = JsonUtility.FromJson<Root>(json);
        foreach (Map map in root.maps)
        {
            GameObject mapObject = new GameObject("New NavMesh");
            mapObject.transform.parent = ARMap.transform;
            mapObject.AddComponent<NavMeshModifier>();
            mapObject.AddComponent<NavMeshObject>();
            mapObject.AddComponent<MeshFilter>();
            mapObject.AddComponent<MeshRenderer>();
            
            mapObject.GetComponent<MeshRenderer>().material = mapMaterial;
            mapObject.GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.0f, 1.0f, 0.1f);

            mapObject.transform.position = map.position;
            mapObject.transform.rotation = Quaternion.Euler(map.rotation);
            mapObject.transform.localScale = map.scale;
            mapObject.GetComponent<MeshFilter>().mesh.vertices = map.meshVertices.ToArray(); 
            mapObject.GetComponent<MeshFilter>().mesh.triangles = map.meshTriangles.ToArray();

            mapList.Add(mapObject);
        }
        foreach (Destination dest in root.destinations){
            GameObject destObject = Instantiate(destination, dest.position, Quaternion.Euler(dest.rotation));
            destObject.transform.localScale = dest.scale;
            for (int i=0; i < 7; i++){
                destObject.transform.GetChild(i).gameObject.GetComponent<TextMesh>().text = dest.textData[i];
            }
            destinationList.Add(destObject);
        }


    }

#if UNITY_IOS
    IEnumerator LoadFeature()
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            yield break;
        }

        var file = File.Open(featurePath, FileMode.Open);
        if (file == null)
        {
            yield break;
        }

        int bytesPerFrame = 1024 * 10;
        var bytesRemaining = file.Length;
        var binaryReader = new BinaryReader(file);
        var allBytes = new List<byte>();
        while (bytesRemaining > 0)
        {
            var bytes = binaryReader.ReadBytes(bytesPerFrame);
            allBytes.AddRange(bytes);
            bytesRemaining -= bytesPerFrame;
            //yield return null;
        }

        var data = new NativeArray<byte>(allBytes.Count, Allocator.Temp);
        data.CopyFrom(allBytes.ToArray());

        Debug.Log(string.Format("Deserializing to ARWorldMap...", featurePath));

        if (ARWorldMap.TryDeserialize(data, out ARWorldMap worldMap))
            data.Dispose();

        if (worldMap.valid)
        {
            statusText.text = "Deserialized successfully.";          
        }
        else
        {
            Debug.LogError("Data is not a valid ARWorldMap.");
            yield break;
        }
        sessionSubsystem.ApplyWorldMap(worldMap);
        
        file.Close();
        statusText.text = "Load";
        yield break;
    }
#endif
}
