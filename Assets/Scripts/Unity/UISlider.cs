using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int sceneLoadIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (!slider)
        {
            if(!TryGetComponent<Slider>(out slider)) 
            {
                Debug.LogWarning("missing slier to load scene");
            }
        }

        StartCoroutine(LoadAsynchronously(sceneLoadIndex));
    }

    IEnumerator LoadAsynchronously (int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        while (!operation.isDone)
        {
            float progess = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progess;
            yield return null;
        }
    }
}
