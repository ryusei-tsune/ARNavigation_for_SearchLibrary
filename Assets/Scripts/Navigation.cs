using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    [SerializeField] Text statusText;
    [SerializeField] InputField searchedKey;
    private LineRenderer line = null;
    private GameObject agent;
    private GameObject[] maps;
    private NavMeshAgent navAgent;
    private NavMeshPath path;

    private void Start()
    {
        agent = GameObject.FindWithTag("MainCamera");
        line = agent.GetComponent<LineRenderer>();
        line.enabled = false;
    }
    public void NavigationButton()
    {
        if (CommonVariables.destinationList.Count > 0)
        {
            maps = GameObject.FindGameObjectsWithTag("MapGameObject");
            foreach (GameObject map in maps)
            {
                map.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
            agent.AddComponent<NavMeshAgent>();
            line.enabled = true;
            navAgent = agent.GetComponent<NavMeshAgent>();
            
            GameObject dest = null;
            string keyword = searchedKey.text;
            foreach(GameObject target in CommonVariables.destinationList){
                if (target.GetComponent<TextMesh>().text.Contains(keyword))
                {
                    dest = target;
                }
            }
            navAgent.radius = 0.1f;
            if (dest != null)
            {
                navAgent.SetDestination(dest.transform.position);
                navAgent.speed = 0.0f;

                path = new NavMeshPath();
                navAgent.CalculatePath(dest.transform.position, path);

                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);

                statusText.text = "キーワード: " + keyword;
            }
            else
            {
                statusText.text = "キーワードと一致する本はありません！";
            }
        }
        else
        {
            statusText.text = "本が登録されていません！";
        }
    }
}
