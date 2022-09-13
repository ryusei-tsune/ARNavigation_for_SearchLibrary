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
    [SerializeField] Text statusText;              //状態をテキストで表示
    ARTrackedImageManager m_TrackedImageManager;        //画像追跡を行うクラス
    public static List<Marker> markerPosList = new List<Marker>(); // 検出したマーカーのリスト
    bool[] markerFlag = new bool[27]; // マーカの個数分

    void Awake()
    {
        // マーカ検出の初期化
        for (int i = 0; i < 27; i++)
        {
            markerFlag[i] = false;
        }
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
            // マーカの名前(ID)を取得
            string markerName = trackedImage.referenceImage.name;
            int num = int.Parse(Regex.Replace(markerName, @"[^0-9]", ""));

            // マーカIDが既に検出済みか確認
            if (!markerFlag[num])
            {
                statusText.text = "Recognize Image";
                // マーカ座標
                Vector3 pos = trackedImage.transform.position;
                Vector3 markerPos = trackedImage.transform.TransformPoint(pos.x, pos.y, pos.z);

                // マーカのID と座標をセットで保持
                Marker marker = new Marker(num, markerPos);
                markerPosList.Add(marker);
                statusText.text = "x: " + markerPos.x + "\ny: " + markerPos.y + "\nz: " + markerPos.z;

                markerFlag[num] = true;
            }
            // ARマーカを読み込むことで現在の階を判断する場合，以下の二つを使い，EM_Loadを一部書き換え
            // CommonVariables.currntFloor = int.Parse(Regex.Replace(markerName, @"[^0-9]", "")); //環境マップを読み込んだ階が何階か判定
            // FileController.GetComponent<EM_Load>().enabled = true;  //EM_Load.csのプログラム起動
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
}