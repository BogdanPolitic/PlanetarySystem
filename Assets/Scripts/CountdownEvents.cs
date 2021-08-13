using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownEvents : MonoBehaviour
{
    public Animator equilibriumCountdown;   // It's actually the FadeManager animator.
    private static CountdownEvents instance;
    private int secondsSinceEquilibrium;
    [SerializeField] private Text countdownSecondsText;

    private void Awake()
    {
        instance = this;
        secondsSinceEquilibrium = 0;
    }

    public static CountdownEvents GetInstance()
    {
        return instance;
    }

    public void ProceedToFadeScreen()
    {
        // If it was the final second of the countdown, this level is won.
        if (secondsSinceEquilibrium == ValueSheet.equilibriumDurationRequirementSeconds)
        {
            LevelParameters.gameWon = true;
            FadeManagement.GetInstance().EndGameProceedToFadeOut();
        }
        secondsSinceEquilibrium++;
        countdownSecondsText.text = (ValueSheet.equilibriumDurationRequirementSeconds - secondsSinceEquilibrium).ToString();

        // Color interpolation:
        float durationPercentage = secondsSinceEquilibrium / (float)ValueSheet.equilibriumDurationRequirementSeconds;
        /*Vector4 newColor = new Vector4(
            ValueSheet.countdownColorStart.x + (ValueSheet.countdownColorEnd.x - ValueSheet.countdownColorStart.x) * durationPercentage,
            ValueSheet.countdownColorStart.y + (ValueSheet.countdownColorEnd.y - ValueSheet.countdownColorStart.y) * durationPercentage,
            ValueSheet.countdownColorStart.z + (ValueSheet.countdownColorEnd.z - ValueSheet.countdownColorStart.z) * durationPercentage,
            ValueSheet.countdownColorStart.w + (ValueSheet.countdownColorEnd.w - ValueSheet.countdownColorStart.w) * durationPercentage
        );*/
        Vector4 newColor = MyMath.InterpolateBetweenVector4s(ValueSheet.countdownColorStart, ValueSheet.countdownColorEnd, durationPercentage);
        countdownSecondsText.color = new Color(newColor.x, newColor.y, newColor.z, newColor.w);
    }

    public void StartEquilibriumCountdown()
    {
        equilibriumCountdown.SetTrigger("CompleteNumberOfPlanets");
        countdownSecondsText.text = ValueSheet.equilibriumDurationRequirementSeconds.ToString();    // Timer reset attached to timer text.
    }

    public void StopEquilibriumCountdown()
    {
        equilibriumCountdown.SetTrigger("IncompleteNumberOfPlanets");
        secondsSinceEquilibrium = 0;    // Timer reset.
    }
}
