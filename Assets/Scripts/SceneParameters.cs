using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneParameters : MonoBehaviour
{
    public enum Scenes
    {
        MENU,
        PLAY
    }
    public static Scenes currentScene;
    public static Dictionary<Scenes, int> sceneIndex;

    private void Awake()
    {
        sceneIndex = new Dictionary<Scenes, int>();
        sceneIndex[Scenes.MENU] = 0;
        sceneIndex[Scenes.PLAY] = 1;
        currentScene = SceneManager.GetActiveScene().buildIndex == sceneIndex[Scenes.MENU]
            ? Scenes.MENU
            : Scenes.PLAY;
    }

    private void Update()
    {
        //Debug.Log("scene = " + currentScene);
    }

    // This function is called immediatly after the fadeOut animation finished (check reference).
    public static void SwitchScene()
    {
        // Switch from PLAY to MENU
        if (currentScene == Scenes.PLAY)
        {
            LevelParameters.changeReturnMenuPath(GenericButtonListener.LOAD_GAME);
            currentScene = Scenes.MENU;
            SceneManager.LoadScene(sceneIndex[Scenes.MENU]);
            return;
        }

        // Switch from MENY to PLAY
        if (currentScene == Scenes.MENU)
        {
            currentScene = Scenes.PLAY;
            SceneManager.LoadScene(sceneIndex[Scenes.PLAY]);
            return;
        }
    }
}
