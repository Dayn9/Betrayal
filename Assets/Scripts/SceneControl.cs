using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private int numTargets;
    private Fade fade;

    [SerializeField] private string nextLevelName;

    [Header("0 for unlimited")]
    [SerializeField] private float timeout = 0;

    private void Awake()
    {
        fade = FindObjectOfType<Fade>();

        if(timeout > 0)
        {
            StartCoroutine(NextLevel(timeout));
        }

        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        numTargets = targets.Length;

        foreach(GameObject target in targets)
        {
            target.GetComponent<Character>().Attacked.AddListener(() =>
            {
                numTargets--;
                Debug.Log(numTargets);
                if(numTargets <= 0)
                {
                    StartCoroutine(NextLevel(1.0f));
                }
            });
        }
    }

    private IEnumerator NextLevel(float delay)
    {
        //wait, then fade out
        yield return new WaitForSeconds(delay);
        fade.FadeOut();

        //wait for fade and then chage scenes
        yield return new WaitForSeconds(fade.FADE_TIME);
        SceneManager.LoadScene(nextLevelName, LoadSceneMode.Single);
    }


}
