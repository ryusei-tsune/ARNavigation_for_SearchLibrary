using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/*
検索結果の表示に利用する bookPanel オブジェクト(Prefab)のボタンを押した際に NavigationButton が発火
このスクリプトは BookPanel 自体に追加しているため，ExtractKeyword 内で出てくる this は BookPanel
this.transform.GetChild(0)は子要素の一番目である Title が取得できる
*/
// public class Navigation : MonoBehaviour
// {
//     private GameObject agent;
//     private GameObject[] maps;
//     private NavMeshAgent navAgent;
//     private NavMeshPath path;

//     public void NavigationButton()
//     {
//         try
//         {
//             ExtractKeyword();
//             GameObject ScrollView = GameObject.FindGameObjectWithTag("ScrollView");
//             ScrollView.SetActive(false);
//             // ユーザー
//             GameObject agent = GameObject.FindWithTag("MainCamera");
//             // ナビゲーションの際に表示する矢印
//             LineRenderer line = agent.GetComponent<LineRenderer>();

//             GameObject dest = null;
//             // ユーザの階と本棚の階が違う場合，エレベータ
//             // 同じ階であれば本棚を目的地にセット
//             if (CommonVariables.currntFloor != BookInformation.floor)
//             {
//                 dest = CommonVariables.movingPointList[0];
//             }
//             else
//             {
//                 // CommonVariables.destinationList は現在配置している本棚のオブジェクト
//                 if (CommonVariables.destinationList.Count > 0)
//                 {
//                     foreach (GameObject target in CommonVariables.destinationList)
//                     {
//                         if (target.GetComponent<TextMesh>().text.Contains(BookInformation.bookCode))
//                         {
//                             dest = target;
//                         }
//                     }
//                 }
//                 else
//                 {
//                     BookSearch.instance.ChangeText("none");
//                     return;
//                 }
//             }

//             // 歩行可能領域をナビゲーションシステムで利用できるようにビルド
//             // ビルドしなければ，システム的には通れないことになっているため，目的地まで案内出来ない
//             maps = GameObject.FindGameObjectsWithTag("MapGameObject");
//             foreach (GameObject map in maps)
//             {
//                 map.GetComponent<NavMeshSurface>().BuildNavMesh();
//             }
//             if (agent.GetComponent<NavMeshAgent>() == null)
//             {
//                 agent.AddComponent<NavMeshAgent>();
//             }
//             line.enabled = true;
//             navAgent = agent.GetComponent<NavMeshAgent>();
//             navAgent.radius = 0.1f;

//             // 目的地がある場合，ナビを表示
//             if (dest != null)
//             {
//                 navAgent.SetDestination(dest.transform.position);
//                 navAgent.speed = 0.0f;

//                 path = new NavMeshPath();
//                 navAgent.CalculatePath(dest.transform.position, path);

//                 line.positionCount = path.corners.Length;
//                 line.SetPositions(path.corners);
//             }
//             else
//             {
//                 BookSearch.instance.ChangeText("failed");
//             }
//         }
//         catch
//         {
//             BookSearch.instance.ChangeText("none");
//         }
//     }

//     // 検索結果から選択した本の情報を抽出
//     private void ExtractKeyword()
//     {
//         BookInformation.bookTitle = this.transform.GetChild(0).GetComponent<Text>().text;
//         BookInformation.bookAuthor = this.transform.GetChild(1).GetComponent<Text>().text;

//         // ex: 場所：津島 中央図書館 西館1F 007.64/S 一般図書
//         string bookPosition = this.transform.GetChild(2).GetComponent<Text>().text;
//         // 1F の1を取得
//         int index = Regex.Matches(bookPosition, @"\d")[0].Index;
//         BookInformation.floor = int.Parse(bookPosition[index].ToString());

//         //本棚のID(007.64/S)を取得
//         string code = bookPosition.Substring(index + 3);
//         BookInformation.bookCode = Regex.Replace(code, @"[^0-9a-zA-Z/.]+", "");

//         if (BookSearch.instance)
//         {
//             BookSearch.instance.ChangeText("search");
//         }
//     }
// }
public class Navigation : MonoBehaviour
{
    private GameObject agent;
    private GameObject[] maps;
    private NavMeshAgent navAgent;
    private NavMeshPath path;
    [SerializeField]
    private InputField InputField;

    public void NavigationButton()
    {
        try
        {
            // ExtractKeyword();
            // GameObject ScrollView = GameObject.FindGameObjectWithTag("ScrollView");
            // ScrollView.SetActive(false);
            // ユーザー
            GameObject agent = GameObject.FindWithTag("MainCamera");
            // ナビゲーションの際に表示する矢印
            LineRenderer line = agent.GetComponent<LineRenderer>();

            GameObject dest = null;

            // CommonVariables.destinationList は現在配置している本棚のオブジェクト
            if (CommonVariables.destinationList.Count > 0)
            {
                foreach (GameObject target in CommonVariables.destinationList)
                {
                    if (target.GetComponent<TextMesh>().text.Contains(InputField.text))
                    {
                        dest = target;
                    }
                }
            }
            else
            {
                BookSearch.instance.ChangeText("none");
                return;
            }

            if (dest == null) {
                dest = CommonVariables.movingPointList[0];
            }

            // 歩行可能領域をナビゲーションシステムで利用できるようにビルド
            // ビルドしなければ，システム的には通れないことになっているため，目的地まで案内出来ない
            maps = GameObject.FindGameObjectsWithTag("MapGameObject");
            foreach (GameObject map in maps)
            {
                map.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
            if (agent.GetComponent<NavMeshAgent>() == null)
            {
                agent.AddComponent<NavMeshAgent>();
            }
            line.enabled = true;
            navAgent = agent.GetComponent<NavMeshAgent>();
            navAgent.radius = 0.1f;

            // 目的地がある場合，ナビを表示
            if (dest != null)
            {
                navAgent.SetDestination(dest.transform.position);
                navAgent.speed = 0.0f;

                path = new NavMeshPath();
                navAgent.CalculatePath(dest.transform.position, path);

                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);
            }
            else
            {
                BookSearch.instance.ChangeText("failed");
            }
        }
        catch
        {
            BookSearch.instance.ChangeText("none");
        }
    }

    // 検索結果から選択した本の情報を抽出
    private void ExtractKeyword()
    {
        BookInformation.bookTitle = this.transform.GetChild(0).GetComponent<Text>().text;
        BookInformation.bookAuthor = this.transform.GetChild(1).GetComponent<Text>().text;

        // ex: 場所：津島 中央図書館 西館1F 007.64/S 一般図書
        string bookPosition = this.transform.GetChild(2).GetComponent<Text>().text;
        // 1F の1を取得
        int index = Regex.Matches(bookPosition, @"\d")[0].Index;
        BookInformation.floor = int.Parse(bookPosition[index].ToString());

        //本棚のID(007.64/S)を取得
        string code = bookPosition.Substring(index + 3);
        BookInformation.bookCode = Regex.Replace(code, @"[^0-9a-zA-Z/.]+", "");

        if (BookSearch.instance)
        {
            BookSearch.instance.ChangeText("search");
        }
    }
}
