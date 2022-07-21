using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
public class EM_Save : MonoBehaviour
{

    [SerializeField] ARSession m_ARSession;
    [SerializeField] InputField inputField;
    [SerializeField] Text statusText;
    [SerializeField] Button saveButton;
    string featurePath;
    string objectPath;
    string markerPath;

    public void SaveButton()
    {
        statusText.enabled = true;
        if (inputField.text == "")
        {
            inputField.placeholder.GetComponent<Text>().color = Color.red;
            inputField.placeholder.GetComponent<Text>().text = "Please input the filename";
        }
        else
        {
            inputField.placeholder.GetComponent<Text>().color = Color.grey;
            inputField.placeholder.GetComponent<Text>().text = "Filename";

            featurePath = Application.persistentDataPath + "/" + inputField.text + "-Feature.ARMap";
            objectPath = Application.persistentDataPath + "/" + inputField.text + "-Object.json";
            markerPath = Application.persistentDataPath + "/" + inputField.text + "-MarkerPos.json";
            if (File.Exists(featurePath))
            {
                File.Delete(featurePath);
            }
            if (File.Exists(objectPath))
            {
                File.Delete(objectPath);
            }
            SaveObject();
#if UNITY_IOS
            StartCoroutine(SaveFeature());
#endif
        }
    }

    private void SaveObject()
    {
        Root root = new Root();
        root.maps = new List<Map>();
        root.destinations = new List<Destination>();

        GameObject mapHolder = GameObject.FindGameObjectWithTag("MapGameObject");
        Transform[] mapMeshs = mapHolder.GetComponentsInChildren<Transform>();

        /*
            環境マップの歩行可能領域(Meshオブジェクト)の保存
        */
        if (mapMeshs.Length > 1)
        {
            for (int i = 1; i < mapMeshs.Length; i++)
            {
                Map map = new Map();

                map.position = mapMeshs[i].transform.position;
                map.rotation = mapMeshs[i].transform.rotation.eulerAngles;
                map.scale = mapMeshs[i].transform.localScale;

                Mesh mapMeshFilter = mapMeshs[i].GetComponent<MeshFilter>().mesh;

                map.meshVertices = new List<Vector3>();
                Vector3[] vertices = mapMeshFilter.vertices;
                for (int j = 0; j < vertices.Length; j++)
                {
                    map.meshVertices.Add(vertices[j]);
                }

                map.meshTriangles = new List<int>();
                int[] triangles = mapMeshFilter.triangles;
                for (int j = 0; j < triangles.Length; j++)
                {
                    map.meshTriangles.Add(triangles[j]);
                }

                root.maps.Add(map);
            }
        }
        /*
            目的地となるオブジェクトの保存
        */
        GameObject[] destinationList = GameObject.FindGameObjectsWithTag("Destination");
        foreach (GameObject dest in destinationList)
        {
            Destination destination = new Destination();

            destination.position = dest.transform.position;
            destination.rotation = dest.transform.rotation.eulerAngles;
            destination.scale = dest.transform.localScale;
            destination.textData = new string[8];
            destination.textData[0] = dest.GetComponent<TextMesh>().text;
            for (int i = 1; i < 8; i++)
            {
                destination.textData[i] = dest.transform.GetChild(i-1).gameObject.GetComponent<TextMesh>().text;
            }
            root.destinations.Add(destination);
        }
        string json;
        try
        {
            using (StreamWriter sw = new StreamWriter(objectPath, false, Encoding.GetEncoding("utf-8")))
            {
                json = JsonUtility.ToJson(root);
                sw.WriteLine(json);
            }

            using (StreamWriter sw = new StreamWriter(markerPath, true, Encoding.GetEncoding("utf-8")))
            {
                List<string> jsonList = new List<string>();
                foreach (var marker in DetectARMarker.markerPosList)
                {
                    jsonList.Add(JsonUtility.ToJson(marker));
                }
                json = "{ \"0\" : [" + string.Join(", ", jsonList) + "]}";
                sw.WriteLine(json);
            }
        }
        catch (Exception e)
        {
            statusText.text = e.ToString();
        }
    }
#if UNITY_IOS
    IEnumerator SaveFeature()
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            statusText.text = "No session subsystem available.\nCould not same.";
            yield break;
        }
        var request = sessionSubsystem.GetARWorldMapAsync();

        while (!request.status.IsDone()) yield return null;

        if (request.status.IsError())
        {
            statusText.text = "Session serialization failed.";
            yield break;
        }
        var worldMap = request.GetWorldMap();

        request.Dispose();
        SaveAndDisposeWorldMap(worldMap);
    }
    void SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        var data = worldMap.Serialize(Allocator.Temp);
        var file = File.Open(featurePath, FileMode.Create);
        var writer = new BinaryWriter(file);
        writer.Write(data.ToArray());
        writer.Close();
        data.Dispose();
        worldMap.Dispose();
        statusText.text = "Save";          //確認用
    }
#endif
}
