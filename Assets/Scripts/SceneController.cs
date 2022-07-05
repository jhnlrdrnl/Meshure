using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void GoToSetup()
    {
        Scene current = SceneManager.GetActiveScene();
        string cur = current.name;

        SceneManager.LoadScene("Setup");

        if (cur == "Main")
            SceneManager.UnloadSceneAsync("Main");
    }

    public void GoToHelp()
    {
        Scene current = SceneManager.GetActiveScene();
        string cur = current.name;

        SceneManager.LoadScene("Help");

        if (cur == "Setup")
            SceneManager.UnloadSceneAsync("Setup");
    }
}
