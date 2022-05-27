using System.Collections;
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
public class EM_FileIO : MonoBehaviour
{

    [SerializeField] ARSession m_ARSession;
    [SerializeField] InputField inputField;
    [SerializeField] Text statusText;
    [SerializeField] Button saveButton;
    string worldMapath;
    string featurePath;
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

            worldMapath = Application.persistentDataPath + "/" + inputField.text + "-test.txt";
            featurePath = Application.persistentDataPath + "/" + inputField.text + "-Feature.ARMap";
            markerPath = Application.persistentDataPath + "/" + inputField.text + "-Object.RowMap";
            if (File.Exists(featurePath))
            {
                File.Delete(featurePath);
            }
#if UNITY_IOS
            StartCoroutine(SaveFeature());
#endif
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
        statusText.text = featurePath;
    }
    void SaveAndDisposeWorldMap(ARWorldMap worldMap)
    {
        var data = worldMap.Serialize(Allocator.Temp);
        var file = File.Open(featurePath, FileMode.Create);
        try
        {
            using (StreamWriter sw = new StreamWriter(worldMapath, false, Encoding.GetEncoding("utf-8")))
            {
                foreach(var tmpdata in data.ToArray()){
                    sw.WriteLine(tmpdata);
                }
            }
        } catch(UnityException e){
            statusText.text = e.ToString();
        }
        var writer = new BinaryWriter(file);
        writer.Write(data.ToArray());
        writer.Close();
        data.Dispose();
        worldMap.Dispose();
        statusText.text = "Save";          //確認用
    }
#endif
}
