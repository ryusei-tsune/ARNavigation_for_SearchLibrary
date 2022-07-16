using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    [SerializeField] Text statusText;
    [SerializeField] InputField searchedKey;
    [SerializeField] LineRenderer line = null;

    public void NavigationButton()
    {
        GameObject agent = GameObject.FindWithTag("MainCamera");
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Destination");

        if (targets.Length != 0)
        {
            GameObject[] maps = GameObject.FindGameObjectsWithTag("MapGameObject");
            foreach (GameObject map in maps)
            {
                map.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
            agent.AddComponent<NavMeshAgent>();
            agent.GetComponent<LineRenderer>().enabled = true;
            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();

            GameObject dest = null;
            string keyword = searchedKey.text;
            foreach (GameObject target in targets)
            {
                if (target.GetComponent<TextMesh>().text.Contains(keyword))
                {
                    dest = target;
                }
            }
            navAgent.radius = 0.2f;
            if (dest != null)
            {
                navAgent.SetDestination(dest.transform.position);
                navAgent.speed = 0.0f;

                NavMeshPath path = new NavMeshPath();
                navAgent.CalculatePath(dest.transform.position, path);

                line.positionCount = path.corners.Length;
                line.SetPositions(path.corners);

                statusText.text = keyword;
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
