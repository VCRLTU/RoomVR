using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

    public void LoadByIndex(int sceneIndex)
    {
        // SceneManager.destroy();
        SceneManager.LoadScene (sceneIndex);
    }
}
