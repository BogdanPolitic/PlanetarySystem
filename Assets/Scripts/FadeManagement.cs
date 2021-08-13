using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// WARNING! This is a prefab object that is instantiated for both PLAY and MENU scenes. You may use gameEndStatus and gameEndStatusText for ONLY the PLAY scene. They're null (unassigned) in the MENU scene.
public class FadeManagement : MonoBehaviour
{
    public Animator fadeAnimator;
    [SerializeField] Animator textChangeTemplateAnimator;
    private static FadeManagement instance;
    public GameObject gameEndStatus;
    Text gameEndStatusText;
    [SerializeField] GameObject finalRewardScreen;

    private void Awake()
    {
        instance = this;
        if (gameEndStatus != null)  // This applies only in the PLAY scene. But since the currentScene variable is also assigned in an Awake() function, we cannot rely on that variable at this frame.
        {
            gameEndStatusText = gameEndStatus.GetComponent<Text>();
        }

        /*GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        int u = 0;
        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<EventSystem>() != null)
            {
                Debug.Log("go name = " + go.transform.name);
                u++;
            }
        }

        if (u == 0) Debug.Log("nu am gasit event system pa");*/
    }

    public static FadeManagement GetInstance()
    {
        return instance;
    }

    // EVENT: Immediatly after the FadeIn animation, this function is called. (see the little event mark at the end (the right side) of the fadeOut clip animation)
    public void GetPlayMode()
    {
        if (SceneParameters.currentScene == SceneParameters.Scenes.PLAY)
        {
            fadeAnimator.SetTrigger("PLAY_mode");
            MainSceneUI.GetInstance().ShowUI();
            Inventory.BeginLevelAccounting();
        }
    }

    // EVENT: Immediatly after the countdown of each second.
    public void EquilibriumDurationAccomplished()
    {
        CountdownEvents.GetInstance().ProceedToFadeScreen();
    }

    // EVENT: Immediatly after the FadeOut animation, this function is called. (see the little event mark at the end (the right side) of the fadeOut clip animation)
    public void ShowEndGameStatus()
    {
        if (SceneParameters.currentScene == SceneParameters.Scenes.PLAY)
        {
            if (MainSceneUI.GetInstance().playerIsRetiringLevel)
            {
                SceneParameters.SwitchScene();
            } else
            {
                fadeAnimator.SetTrigger("ShowEndGameStatus");
            }
        }
        else if (SceneParameters.currentScene == SceneParameters.Scenes.MENU)
            SceneParameters.SwitchScene();
    }

    // Proceed to fade out from the MENU scene.
    public void MenuProceedToFadeOut()
    {
        fadeAnimator.SetTrigger("ProceedToFadeOut");
    }

    // The level is lost or won, when this function is called.
    public void EndGameProceedToFadeOut()
    {
        MainSceneUI.GetInstance().HideUI();
        fadeAnimator.SetTrigger("ProceedToFadeOut");

        gameEndStatusText.text = LevelParameters.gameWon
            ? ValueSheet.gameWonString
            : ValueSheet.gameLostString;

        if (LevelParameters.gameWon)
            if (LevelParameters.currentLevel > LevelParameters.maxReachedLevel)
            {
                LevelParameters.maxReachedLevel = LevelParameters.currentLevel;
                Debug.Log("game won so max reached level = " + LevelParameters.maxReachedLevel);
            }
    }

    // EVENT: Immediatly after the ShowEndGameStatus animation, this function is called. (see the little event mark at the end (the right side) of the fadeOut clip animation)
    public void SwitchSceneOrClaimRewards()
    {
        if (LevelParameters.gameWon)
        {
            fadeAnimator.SetTrigger("ClaimRewards");
            finalRewardScreen.SetActive(true);
            Inventory.CalculateLevelRewards();
        }
        else
            SceneParameters.SwitchScene();
    }

    public void ProceedToClaimRewards()
    {
        textChangeTemplateAnimator.SetTrigger("PlayerStartsReadingEarntPlanets");
    }

    public void SwitchScene()
    {
        SceneParameters.SwitchScene();
    }

    // EVENT: Immediatly after the waiting time for the user to read the number of planets earnt for each stack. As for the time I wrote this, this time duration is 3 seconds ("PlayerReadsTheRewards" animation).
    public void StartTemplateChanging()
    {
        textChangeTemplateAnimator.SetTrigger("StartTemplateChanging");
        AnimatedRewardClaims.GetInstance().TemplateChangingStarted();
    }

    private void Update()
    {
        //if (fadeAnimator != null)
        //    Debug.Log("curr anim = " + fadeAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);  
        //    It works ONLY if the current state has an animation assigned.
        //    It does not work on empty states, it gives "Index out of range" exception.
    }
}
