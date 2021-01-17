using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    int previousTouchCount;
    Coroutine changeCoroutine;
    int loadIndex;

    // Update is called once per frame
    void Update()
    {
        var isChange = false;

        if (Input.GetMouseButtonDown(0))
        {
            isChange = true;
        }
        if (previousTouchCount == 0 && Input.touchCount == 1)
        {
            isChange = true;
        }

        previousTouchCount = Input.touchCount;

        if (isChange && changeCoroutine == null)
        {
            changeCoroutine = StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene()
    {
        var sceneIndices = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings)
            .Where(x => x != SceneManager.GetSceneByName("SceneChanger").buildIndex)
            .ToArray();

        var unloadOperations = sceneIndices
            .Select(x => SceneManager.GetSceneByBuildIndex(x))
            .Where(x => x.isLoaded)
            .Select(x => SceneManager.UnloadSceneAsync(x))
            .ToArray();
        foreach (var unloadOperation in unloadOperations)
        {
            yield return unloadOperation;
        }

        var loadSceneIndex = sceneIndices[loadIndex];
        yield return SceneManager.LoadSceneAsync(loadSceneIndex, LoadSceneMode.Additive);
        loadIndex = (loadIndex + 1) % sceneIndices.Length;

        changeCoroutine = null;
    }
}
