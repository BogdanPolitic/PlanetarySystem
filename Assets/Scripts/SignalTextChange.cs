using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalTextChange : MonoBehaviour
{
    [SerializeField] GameObject finalRewardScreen;

    // EVENT: after each end of the text template animation, this is called.
    public void SignalChange()
    {
        if (finalRewardScreen.activeSelf)
            AnimatedRewardClaims.GetInstance().TextChanged();
    }

    // EVENT: Immediatly after the player reads the reward planets earnt (the "PlayerReadsTheRewards" animation).
    public void StartTextChanging()
    {
        FadeManagement.GetInstance().StartTemplateChanging();
    }

    // EVENT: In the middle of the change the text will have the localScale equal to (1, 1, 1), this is when text changing should stop if the reward has been consumed (= 0).
    public void InTheMiddleOfTheChange()
    {
        AnimatedRewardClaims.GetInstance().MiddleOfTheTextChange();
    }
}
