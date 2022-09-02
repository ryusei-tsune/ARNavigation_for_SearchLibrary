using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    private GameObject agent;
    private GameObject[] maps;
    private NavMeshAgent navAgent;
    private NavMeshPath path;

    public void NavigationButton()
    {
        ExtractKeyword();
        GameObject ScrollView = GameObject.FindGameObjectWithTag("ScrollView");
        ScrollView.SetActive(false);
        GameObject agent = GameObject.FindWithTag("MainCamera");
        LineRenderer line = agent.GetComponent<LineRenderer>();

        GameObject dest = null;
        if (CommonVariables.currntFloor != BookInformation.floor)
        {
            dest = GameObject.FindGameObjectWithTag("Elevator");
        }
        else
        {
            if (CommonVariables.destinationList.Count > 0)
            {
                foreach (GameObject target in CommonVariables.destinationList)
                {
                    if (target.GetComponent<TextMesh>().text.Contains(BookInformation.bookCode))
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
        }

        maps = GameObject.FindGameObjectsWithTag("MapGameObject");
        foreach (GameObject map in maps)
        {
            map.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        agent.AddComponent<NavMeshAgent>();
        line.enabled = true;
        navAgent = agent.GetComponent<NavMeshAgent>();
        navAgent.radius = 0.1f;

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

    private void ExtractKeyword()
    {
        BookInformation.bookTitle = this.transform.GetChild(0).GetComponent<Text>().text;
        BookInformation.bookAuthor = this.transform.GetChild(1).GetComponent<Text>().text;

        string bookPosition = this.transform.GetChild(2).GetComponent<Text>().text;
        int index = Regex.Matches(bookPosition, @"\d")[0].Index;
        BookInformation.floor = int.Parse(bookPosition[index].ToString());

        string code = bookPosition.Substring(index + 3);
        BookInformation.bookCode = Regex.Replace(code, @"[^0-9a-zA-Z/.]+", ""); //本棚のIDを取得

        if (BookSearch.instance)
        {
            BookSearch.instance.ChangeText("search");
        }
    }
}
