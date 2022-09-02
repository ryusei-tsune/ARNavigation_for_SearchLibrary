using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BookSearch : MonoBehaviour
{
    public static BookSearch instance = null;
    private string fetchDataUrl = "https://scraping-okadai-library.herokuapp.com/api/scraping";
    private string fetchDetailUrl = "https://scraping-okadai-library.herokuapp.com/api/detail-information";
    private GameObject ScrollView;
    private GameObject panelContent;
    [SerializeField] Text statusText;
    [SerializeField] InputField searchWord;
    [SerializeField] GameObject bookPanel;
    private List<Book> bookList = new List<Book>();

    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GameObject agent = GameObject.FindWithTag("MainCamera");
        LineRenderer line = agent.GetComponent<LineRenderer>();
        line.enabled = false;

        panelContent = GameObject.FindGameObjectWithTag("PanelContent");
        ScrollView = GameObject.FindGameObjectWithTag("ScrollView");
        ScrollView.SetActive(false);
    }

    public void SearchKeyword()
    {
        try
        {
            foreach (Transform panel in panelContent.GetComponentInChildren<Transform>())
            {
                Destroy(panel.gameObject);
            }
            bookList.Clear();
            StartCoroutine(FetchData());
        }
        catch (Exception e)
        {
            statusText.text = "Error" + e.Message;
        }
    }

    IEnumerator FetchData()
    {
        string keyword = searchWord.text;
        searchWord.text = "";
        WWWForm form = new WWWForm();
        form.AddField("name", keyword);
        UnityWebRequest request = UnityWebRequest.Post(fetchDataUrl, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string text = request.downloadHandler.text;
            while (text.Contains("{"))
            {
                string tempInfo = text.Substring(text.IndexOf("{"), text.IndexOf("}") - text.IndexOf("{") + 1);
                text = text.Remove(text.IndexOf("{"), text.IndexOf("}") - text.IndexOf("{") + 1);
                Book book = JsonUtility.FromJson<Book>(tempInfo);
                bookList.Add(book);
            }

            ScrollView.SetActive(true);

            /*
            ///////////////////////////////////////////////
            パネルへの表示方法　要検討
            ///////////////////////////////////////////////
            */
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

    public void ChangeText(string type)
    {
        switch (type) {
            case "search":
                statusText.text = BookInformation.bookTitle + "\n\n" + BookInformation.bookAuthor + "\n\n" + "所蔵：" + BookInformation.floor + "F, " + BookInformation.bookCode;
                break;
            case "none":
                statusText.text = "この階に本棚は登録されていません";
                break;
            case "failed":
                statusText.text = "お探しの本は見つかりませんでした";
                break;
            default:
                break;
        }
    }

    IEnumerator FetchDetail()
    {
        WWWForm form = new WWWForm();
        // 変更したい
        form.AddField("url", this.GetComponent<Text>().text);
        UnityWebRequest request = UnityWebRequest.Post(fetchDetailUrl, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Infomation InfomationData = JsonUtility.FromJson<Infomation>(request.downloadHandler.text);

            BookInformation.floor = int.Parse(Regex.Replace(InfomationData.Floor, @"[^0-9]+", "")); // 本棚が何階にあるか判定
            InfomationData.Location = Regex.Replace(InfomationData.Location, @"/{2,}", "");
            BookInformation.bookCode = Regex.Replace(InfomationData.Location, @"/$", ""); //本棚のIDを取得
            //　ここも変えたい
            BookInformation.bookTitle = this.transform.parent.gameObject.transform.parent.gameObject.GetComponent<Text>().text;

            // foreach (Transform n in Panel.transform)
            // {
            //     GameObject.Destroy(n.gameObject);
            // }
            // Panel.SetActive(false);
        }
    }
}
