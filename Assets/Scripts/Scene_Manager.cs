using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    private int Sceneflag = 0;

    public void changeScene()
    {
        if (Sceneflag == 0)
        {
            SceneManager.LoadScene("LEM_NaviScene", LoadSceneMode.Single);
            Sceneflag = 1;
        }
    }
}
