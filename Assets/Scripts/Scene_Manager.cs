using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{

    public void OnClickAdmin()
    {
        SceneManager.LoadScene("AdminScene", LoadSceneMode.Single);

    }

    public void OnClickUser()
    {
        SceneManager.LoadScene("UserScene", LoadSceneMode.Single);

    }

    public void OnClickReturn()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

}
