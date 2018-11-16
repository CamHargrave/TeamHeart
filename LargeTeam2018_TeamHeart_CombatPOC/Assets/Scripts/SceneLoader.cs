using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{

    public void LoadSceneOnClick(int sceneNo)
    {
        SceneManager.LoadScene(sceneNo);
    }

}