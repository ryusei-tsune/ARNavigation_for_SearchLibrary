using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif
public class EM_Load : MonoBehaviour
{
    public ARSession m_ARSession;
    [SerializeField] Text statusText;
    [SerializeField] GameObject mapInstantiate; //環境マップの基となるオブジェクト
    [SerializeField] Material mapMaterial; // 歩行可能領域の色
    [SerializeField] GameObject destination; // 本棚のオブジェクト
    [SerializeField] GameObject elevator; // 移動地点のオブジェクト
    // [SerializeField] Dropdown fileSelector; // 始めにロードする環境マップを選択
    private string featurePath;
    private string objectPath;
    private GameObject ARMap;
    private List<GameObject> mapList = new List<GameObject>();
    public static GameObject agent;
    public static LineRenderer line;
    private void Start()
    {
        agent = GameObject.FindWithTag("MainCamera");
        line = agent.GetComponent<LineRenderer>();
        line.enabled = false;

        // 初期位置が変化する場合，ユーザが環境マップを選択する必要があり・・・84~86行目を使用
        // GetDropdownList();
        statusText.text = "ロードボタンを押してね！";
        // if (SearchInformation.floor != -1){
        //     LoadButton();
        // }
    }

    //     private void GetDropdownList()
    //     {
    //         // /dataPath/に存在するファイル名をfileSelectorに追加
    // #if UNITY_EDITOR
    //         string[] tempFileLists = Directory.GetFiles(Application.dataPath + "/");
    // #elif UNITY_IPHONE
    //         string[] tempFileLists = Directory.GetFiles(Application.persistentDataPath + "/");
    // #endif

    //         List<string> dropOptions = new List<string>();

    //         // ファイル名を 'dataPath/~' から '~' に変更
    //         foreach (string filename in tempFileLists)
    //         {
    //             string temp;
    // #if UNITY_EDITOR
    //             temp = filename.Replace(Application.dataPath + "/", "");
    // #elif UNITY_IPHONE
    //             temp = filename.Replace(Application.persistentDataPath + "/", "");
    // #endif
    //             if (Regex.IsMatch(temp, @".json|.ARMap")) dropOptions.Add(temp.Substring(0, Regex.Matches(temp, @".json|.ARMap")[0].Index));   //ファイル名を一時的に格納
    //         }

    //         dropOptions = dropOptions.Distinct().ToList<string>();
    //         fileSelector.ClearOptions();
    //         fileSelector.AddOptions(dropOptions);
    //     }

    public void LoadButton()
    {
        if (SearchInformation.floor == -1)
        {
            // 開始時に環境マップをロード．初期位置が決まっている場合
            featurePath = Application.persistentDataPath + "/3.ARMap";
            objectPath = Application.persistentDataPath + "/3.json";
            CommonVariables.currntFloor = 3;
            statusText.text = "現在位置 : " + CommonVariables.currntFloor + "階\n" + "キーワードを入力して，本を検索してね！";
            // featurePath = Application.persistentDataPath + "/" + fileSelector.captionText.text + ".ARMap";
            // objectPath = Application.persistentDataPath + "/" + fileSelector.captionText.text + ".json";
            // CommonVariables.currntFloor = int.Parse(Regex.Replace(fileSelector.captionText.text, @"[^0-9]+", ""));

        }
        else
        {
            // 検索結果を基に該当する階に移動した後，環境マップをロード
            featurePath = Application.persistentDataPath + "/" + SearchInformation.floor + ".ARMap";
            objectPath = Application.persistentDataPath + "/" + SearchInformation.floor + ".json";
            CommonVariables.currntFloor = SearchInformation.floor;
            statusText.text = "現在位置 : " + CommonVariables.currntFloor + "階\n" + "ナビボタンを押してね！";
        }
        if (File.Exists(featurePath))
        {
#if UNITY_IOS
            StartCoroutine(LoadFeature(featurePath, objectPath));
#endif
        }
    }

    private void LoadObject(string path)
    {
        ARMap = Instantiate(mapInstantiate);
        string json = "";
        try
        {
            json = File.ReadAllText(path);

            // 環境マップの情報を利用可能なように分解していく
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
            foreach (Destination dest in root.destinations)
            {
                GameObject destObject = Instantiate(destination, dest.position, Quaternion.Euler(dest.rotation));
                destObject.transform.localScale = dest.scale;
                destObject.GetComponent<TextMesh>().text = dest.textData[0];
                for (int i = 1; i < 8; i++)
                {
                    destObject.transform.GetChild(i - 1).gameObject.GetComponent<TextMesh>().text = dest.textData[i];
                }
                destObject.SetActive(false);
                CommonVariables.destinationList.Add(destObject);
            }

            foreach (MovingPoint movingPoint in root.movingPoints)
            {
                GameObject elevatorObject = Instantiate(elevator, movingPoint.position, Quaternion.Euler(movingPoint.rotation));
                elevatorObject.transform.localScale = movingPoint.scale;
                CommonVariables.movingPointList.Add(elevatorObject);
                elevatorObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            statusText.text = e?.Message;
        }
    }

#if UNITY_IOS
    IEnumerator LoadFeature(string feature, string obje)
    {
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
        if (sessionSubsystem == null)
        {
            yield break;
        }

        var file = File.Open(feature, FileMode.Open);
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

        Debug.Log(string.Format("Deserializing to ARWorldMap...", feature));

        if (ARWorldMap.TryDeserialize(data, out ARWorldMap worldMap))
            data.Dispose();

        if (!worldMap.valid)
        {
            Debug.LogError("Data is not a valid ARWorldMap.");
            yield break;
        }
        sessionSubsystem.ApplyWorldMap(worldMap);

        file.Close();

        if (File.Exists(obje))
        {
            LoadObject(obje);
        }
    }
#endif

    // 階を移動する場合，前の環境マップを削除する際に使用
    public void ResetButton()
    {
        try
        {
            ARSession new_ARSession = new ARSession();
            m_ARSession = new_ARSession;
            m_ARSession.Reset();
            Destroy(ARMap);
            mapList.Clear();
            foreach (GameObject target in CommonVariables.destinationList)
            {
                Destroy(target);
            }
            CommonVariables.destinationList.Clear();
            foreach (GameObject target in CommonVariables.movingPointList)
            {
                Destroy(target);
            }
            CommonVariables.movingPointList.Clear();
            line.enabled = false;
            SceneManager.LoadScene("UserScene", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            statusText.text = e.Message;
        }
    }

    private void Update()
    {
#if UNITY_IOS
        var sessionSubsystem = (ARKitSessionSubsystem)m_ARSession.subsystem;
#else
        XRSessionSubsystem sessionSubsystem = null;
#endif
        if (sessionSubsystem == null)
            return;
    }
}
