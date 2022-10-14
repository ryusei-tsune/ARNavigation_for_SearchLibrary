using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{

    public void OnClickAdmin()
    {
        SceneManager.LoadScene("CEM_DMScene", LoadSceneMode.Single);

    }

    public void OnClickUser()
    {
        SceneManager.LoadScene("LEM_NaviScene", LoadSceneMode.Single);

    }

    public void OnClickDemo()
    {
        SceneManager.LoadScene("DemoScene", LoadSceneMode.Single);

    }

    public void OnClickReturn()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

}
