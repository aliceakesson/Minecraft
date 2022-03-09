using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{

    public GameObject loadingScreenPanel;
    public GameObject menu; 
    public Slider progressbar; 

    public void LoadLevel()
    {
        StartCoroutine(LoadAsynchronously());
        
    }


    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        menu.SetActive(false);
        loadingScreenPanel.SetActive(true);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressbar.value = progress;
            Debug.Log(progressbar.value);
            yield return null; 

        }
    }

}
