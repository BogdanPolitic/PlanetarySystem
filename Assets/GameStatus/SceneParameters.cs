using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneParameters : MonoBehaviour
{
    // static parameters:
    public static int currentLevel = 0;
    public static float mapDimension = 10.0f;
    public static float totalTime = 20.0f;
    public static int requiredNumberOfPlanets = 5;
    float internalTime;
    public static int secondsLeft;

    public static int returnMenuPath;

    // dynamic:
    public static Inventory.PlanetStack currentPlanet;

    private void Start()
    {
        Inventory.initializeInventory();
        currentPlanet = RandomPlanetSelector.selectRandomPlanet();

        internalTime = 0f;

        returnMenuPath = 0;
    }

    // the currentPlanet will become the next randomly selected planet (selected by the RNG), but the current (not the next) planet is returned by the method:
    public static Inventory.PlanetStack selectCurrentPlanetAndGoToNext()
    {
        Inventory.PlanetStack currentOne = currentPlanet;
        currentPlanet = RandomPlanetSelector.selectRandomPlanet();

        return currentOne;
    }

    public void Update()
    {
        if (MainSceneUI.gameIsPaused())
            return;

        internalTime += Time.deltaTime;
        if (internalTime < totalTime)
            secondsLeft = (int)(totalTime - internalTime);
        else
        {
            //GameObject.Find("CameraFade").GetComponent<EndSceneFade>().startFadingAndGoToMenu();
            changeReturnMenuPath(GenericButtonListener.LOAD_GAME);
            SceneManager.LoadScene(0);
        }
    }

    public void changeReturnMenuPath(int menuPath)
    {
        returnMenuPath = menuPath;
    }
}
