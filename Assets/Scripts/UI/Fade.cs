using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    private SpriteRenderer Render { get; set; }
    private Color color = Color.black;

    public float FADE_TIME { get; } = 2.0f;

    private void Awake()
    {
        Render = GetComponent<SpriteRenderer>();
        Render.color = Color.black;
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(Lerp(0, FADE_TIME));
    }

    public void FadeOut()
    {
        StartCoroutine(Lerp(1, FADE_TIME));
    }

    private IEnumerator Lerp(float a, float time)
    {
        float t = 0, p = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            p = t / time;

            color.a = Mathf.Lerp(color.a, a, p);
            Render.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
}
