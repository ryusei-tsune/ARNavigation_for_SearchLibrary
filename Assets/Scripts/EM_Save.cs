using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    string worldMapath;
    string worldMapath1;
    string worldMapath2;
    string featurePath;
    string objectPath;
    // List<ARSession> ARsessions;

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

            worldMapath = Application.persistentDataPath + "/" + inputField.text + "-test.txt";
            worldMapath1 = Application.persistentDataPath + "/" + inputField.text + "-test1.txt";
            worldMapath2 = Application.persistentDataPath + "/" + inputField.text + "-test2.txt";
            featurePath = Application.persistentDataPath + "/" + inputField.text + "-Feature.ARMap";
            objectPath = Application.persistentDataPath + "/" + inputField.text + "-Object.RowMap";
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

    private void SaveObject() {
        Root root = new Root();
        root.maps = new List<Map>();
        root.landMarks = new List<LandMark>();

        GameObject mapHolder = GameObject.FindGameObjectWithTag("MapGameObject");
        Transform[] mapMeshs = mapHolder.GetComponentsInChildren<Transform>();

        if (mapMeshs.Length > 1){
            for (int i=1; i < mapMeshs.Length; i++){
                Map map = new Map();
                
                map.position = mapMeshs[i].transform.position;
                map.rotation = mapMeshs[i].transform.rotation.eulerAngles;
                map.scale = mapMeshs[i].transform.localScale;

            }
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
