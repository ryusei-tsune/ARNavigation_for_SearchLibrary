using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;


/// <summary>
/// ARマーカの座標を検出及び保存(後々座標変換を行う際に使用)
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class DetectARMarker : MonoBehaviour
{
    /// <summary>
    /// The prefab has a world space UI canvas,
    /// which requires a camera to function properly.
    /// </summary>
    [SerializeField] Camera m_WorldSpaceCanvasCamera;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>

    [SerializeField] InputField inputField;
    [SerializeField] Text statusText;              //状態をテキストで表示

    ARTrackedImageManager m_TrackedImageManager;        //画像追跡を行うクラス
    Dictionary<string, Vector3> markerPosList;
    bool[] marker = { false, false };

    void Awake()
    {
        // marker = new bool[2];
        // marker = Enumerable.Repeat<bool>(false, 2).ToArray();
        markerPosList = new Dictionary<string, Vector3>();
        statusText.enabled = false;
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }


    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        statusText.enabled = true;
        //画像を認識した際に実行
        if ((trackedImage.trackingState != TrackingState.None))
        {
            string markerName = trackedImage.referenceImage.name;
            int num = int.Parse(Regex.Replace(markerName, @"[^0-9]", ""));

            if (!marker[num])
            {
                statusText.text = "Recognize Image";
                Vector3 pos = trackedImage.transform.position;
                Vector3 markerPos = trackedImage.transform.TransformPoint(pos.x, pos.y, pos.z);
                //認識した画像の名前を文字列に
                try
                {
                    markerPosList.Add(num.ToString(), markerPos);
                    statusText.text = "x: " + markerPos.x + "\ny: " + markerPos.y + "\nz: " + markerPos.z;
                }
                catch (Exception e)
                {
                    statusText.text = "already" + e;
                }

                //VariableCP.currentfloor = int.Parse(Regex.Replace(filename, @"[^0-9]", "")); //環境マップを読み込んだ階が何階か判定
                //FileController.GetComponent<UFileIO>().enabled = true;  //UFileIO.csのプログラム起動
                marker[num] = true;
            }
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {

        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);

            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateInfo(trackedImage);
        }

    }

    public void PositionSave()
    {
        if (inputField.text != "")
        {
            string filename = Application.persistentDataPath + "/" + inputField.text + "-MarkerPos.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(filename, true, Encoding.GetEncoding("utf-8")))
                {
                    foreach (var marker in markerPosList)
                    {
                        string json = marker.Key + ": (" + marker.Value.x + ", " + marker.Value.y + ", " + marker.Value.z + ")";
                        sw.WriteLine(json);
                    }
                }
            }
            catch (Exception e)
            {
                statusText.text = e.ToString();
            }
        }
    }
}