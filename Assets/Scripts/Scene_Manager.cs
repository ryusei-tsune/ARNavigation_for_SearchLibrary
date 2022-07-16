using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    private int Sceneflag = 0;

    public void OnClickAdmin()
    {
        if (Sceneflag == 0)
        {
            Sceneflag = 1;
            SceneManager.LoadScene("CEM_DMScene", LoadSceneMode.Single);
        }
    }

    public void OnClickUser()
    {
        if (Sceneflag == 0)
        {
            Sceneflag = 1;
            SceneManager.LoadScene("LEM_NaviScene", LoadSceneMode.Single);
        }
    }

    public void OnClickReturn()
    {
        if (Sceneflag == 1)
        {
            Sceneflag = 0;
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }
    }

}
