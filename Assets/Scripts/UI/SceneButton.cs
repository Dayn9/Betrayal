using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneButton : UIButton
{
    [SerializeField] private string sceneName;

    protected override void OnActive() { }

    protected override void OnClick()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    protected override void OnEnter() { }
}