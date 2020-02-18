using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private int numTargets;
    private Fade fade;

    [SerializeField] private string nextLevelName; //level to transition to 

    [Header("0 for unlimited")]
    [SerializeField] private float timeout = 0; //time to next scene

    private void Awake()
    {
        fade = FindObjectOfType<Fade>();

        //If there is a timeout, Start the countdown
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
                if(numTargets <= 0)
                {
                    //Go to the next level after 1 second
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
