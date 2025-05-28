using UnityEngine;

using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    void Start()
    {
        // Load the Menu scene after 1 second
        Invoke("LoadMain", 1f);
    }

    void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }
}
