using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BookSearch : MonoBehaviour
{
    // 他のクラス(Navigationクラス)でBookSearchクラスの関数を呼び出すため
    public static BookSearch instance = null;
    // スクレイピングを行う API のエンドポイント
    private string fetchDataUrl = "https://scraping-okadai-library.herokuapp.com/api/scraping";
    // private string fetchDataUrl = "https://scraping-okadai-library.onrender.com/api/scraping"; //念のためスクレイピングアプリを別のサービスでもホスティング
    private GameObject ScrollView;
    private GameObject panelContent;
    [SerializeField] Text statusText;
    [SerializeField] InputField searchWord;
    [SerializeField] GameObject bookPanel; // 検索結果1つを表示
    private List<Book> bookList = new List<Book>();

    public void Awake()
    {
        // instance を今作成されているBookSearchクラスに
        instance = this;
    }
    void Start()
    {
        panelContent = GameObject.FindGameObjectWithTag("PanelContent"); //ScrollViewオブジェクトの子コンポーネント(Content)
        ScrollView = GameObject.FindGameObjectWithTag("ScrollView"); //ScrollViewオブジェクト本体
        ScrollView.SetActive(false);
    }

    // 検索ボタンが押されたときに発火
    public void SearchKeyword()
    {
        try
        {
            // 前の検索結果が画面に表示されている場合，一度削除
            foreach (Transform panel in panelContent.GetComponentInChildren<Transform>())
            {
                Destroy(panel.gameObject);
            }
            bookList.Clear();
            StartCoroutine(FetchData());
        }
        catch (Exception e)
        {
            statusText.text = "Error: " + e.Message;
        }
    }

    // スクレイピングのAPIをたたく
    IEnumerator FetchData()
    {
        string keyword = searchWord.text;
        searchWord.text = "";
        WWWForm form = new WWWForm();
        form.AddField("name", keyword);
        // ここで通信
        UnityWebRequest request = UnityWebRequest.Post(fetchDataUrl, form);　
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            // Jsonの配列形式の文字列を受け取る "[{...},{...}]"
            string text = request.downloadHandler.text;
            while (text.Contains("{"))
            {
                // Json１つ分 {...} を切り出す
                string tempInfo = text.Substring(text.IndexOf("{"), text.IndexOf("}") - text.IndexOf("{") + 1);
                text = text.Remove(text.IndexOf("{"), text.IndexOf("}") - text.IndexOf("{") + 1);
                Book book = JsonUtility.FromJson<Book>(tempInfo);
                bookList.Add(book);
            }

            ScrollView.SetActive(true);

            // 検索結果の一覧を表示
            foreach (Book book in bookList)
            {
                GameObject panel = Instantiate(bookPanel);
                panel.transform.GetChild(0).GetComponent<Text>().text = book.name;
                panel.transform.GetChild(1).GetComponent<Text>().text = "著者：" + book.author;
                panel.transform.GetChild(2).GetComponent<Text>().text = "場所：" + book.position;
                panel.transform.SetParent(panelContent.transform);
            }
        }
    }

    // Navigationクラスで呼ばれ，画面上のテキストの文字を編集
    public void ChangeText(string type)
    {
        switch (type) {
            case "search":
                statusText.text = BookInformation.bookTitle + "\n\n" + BookInformation.bookAuthor + "\n\n" + "所蔵：" + BookInformation.floor + "F, " + BookInformation.bookCode;
                break;
            case "none":
                // statusText.text = "この階に本棚は登録されていません";
                statusText.text = CommonVariables.currntFloor + " != " + BookInformation.floor + " = " + (CommonVariables.currntFloor != BookInformation.floor).ToString() + "   " + CommonVariables.destinationList.Count + " Test " + CommonVariables.movingPointList.Count;
                break;
            case "failed":
                statusText.text = "お探しの本は見つかりませんでした";
                break;
            default:
                break;
        }
    }
    public void ChangeText(string type, string error)
    {
        switch (type) {
            case "error":
                statusText.text = "Error: " + error;
                break;
            default:
                statusText.text = "Error: unknown";
                break;
        }
    }
}
