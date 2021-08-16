using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelParameters : MonoBehaviour
{

    // static parameters:
    public static int currentLevel = 0;
    public static int maxReachedLevel = -1;
    public static int totalPoints = 0;
    public static float mapDimension = 10.0f;
    public static float totalTime = 100.0f;
    public static int requiredNumberOfPlanets = 5;
    float internalTime;
    public static int secondsLeft;
    public static int numberOfPlanetsLeft = -1;

    public static int returnMenuPath;
    public static bool equilibriumActivated;    // While the total number of required planets is satisfied, this is set to true.

    public static bool gameWon; // If it's true, it DOESN'T necessarily mean that the level is won, but that the required number of planets is satisifed in the current frame.
    // If the countdown reaches the end and gameWon is still true, then the level is really won.

    public static bool planetPlacedThisFrame;
    public static int planetsDestroyedThisFrame;
    int numberOfPlanetsLastFrame;

    // dynamic:
    public static Inventory.PlanetStack currentPlanet;
    public static int numberOfSwitchTokens;

    private void Awake()
    {
        totalPoints = 0;
        planetPlacedThisFrame = false;
        numberOfPlanetsLastFrame = 0;
        planetsDestroyedThisFrame = 0;
        numberOfSwitchTokens = ValueSheet.numberOfSwitchTokens;
    }

    private void Start()
    {
        Inventory.initializeInventory();
        currentPlanet = RandomPlanetSelector.selectRandomPlanet();

        internalTime = 0f;

        returnMenuPath = 0;
        equilibriumActivated = false;
        gameWon = false;
    }

    // The currentPlanet will become the next randomly selected planet (selected by the RNG), but the current (not the next) planet is returned by the method:
    public static Inventory.PlanetStack selectCurrentPlanetAndGoToNext()
    {
        Inventory.PlanetStack currentOne = currentPlanet;
        currentPlanet = RandomPlanetSelector.selectRandomPlanet();

        return currentOne;
    }

    public void Update()
    {
        if (MainSceneUI.GameIsPaused())
            return;

        internalTime += Time.deltaTime;
        if (internalTime < totalTime)
            secondsLeft = (int)(totalTime - internalTime);


        if (numberOfPlanetsLeft > 0 && equilibriumActivated)
        {
            CountdownEvents.GetInstance().StopEquilibriumCountdown();
            equilibriumActivated = false;
        }

        if (numberOfPlanetsLeft <= 0 && !equilibriumActivated)
        {
            equilibriumActivated = true;
            CountdownEvents.GetInstance().StartEquilibriumCountdown();  // After (10) seconds, the game is won if equilibrium doesn't break.
        }

        if (internalTime > totalTime && !equilibriumActivated)
        {
            // GAME LOST
            FadeManagement.GetInstance().EndGameProceedToFadeOut();
        }


        planetPlacedThisFrame = (numberOfPlanetsLastFrame < PathReader1.memory.Count);
        planetsDestroyedThisFrame = Mathf.Max(numberOfPlanetsLastFrame - PathReader1.memory.Count, 0);
        numberOfPlanetsLastFrame = PathReader1.memory.Count;

        if (planetPlacedThisFrame)
        {
            Notifications.AddNotification(Notifications.NotificationType.PLANET_PLACED);
            totalPoints += ValueSheet.pointsPerPlanetPlaced;
            MainSceneUI.GetInstance().SignalTotalPointsChange();
        }

        if (planetsDestroyedThisFrame > 0)
        {
            Notifications.AddNotification(Notifications.NotificationType.PLANET_DESTROYED);
            totalPoints -= planetsDestroyedThisFrame * ValueSheet.pointsPenaltyPerPlanetDestroyed;
            MainSceneUI.GetInstance().SignalTotalPointsChange();
        }

        totalPoints = Mathf.Max(totalPoints, 0);
    }

    public static void changeReturnMenuPath(int menuPath)
    {
        returnMenuPath = menuPath;
    }

    // This is called only if the game is won. The rewards will be displayed in the reward claming screen.
    public static void CalculateRewards()
    {

    }
}
