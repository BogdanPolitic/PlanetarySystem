using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberOfSecondsLeftTextEvents : MonoBehaviour
{
    /*public static NumberOfSecondsLeftTextEvents instance;

    Animator anim;
    AnimatorOverrideController animOverrideController;
    AnimationClip animationClip;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        AnimationCurve changeColor = AnimationCurve.Linear(0.0f, 0.0f, LevelSpecification.levelSpecs[0].totalTime * 0 + 5.0f, 1.0f);
        animationClip = new AnimationClip();
        animationClip.SetCurve("", typeof(Text), "Text.Color", changeColor);

        anim = GetComponent<Animator>();
        animOverrideController = new AnimatorOverrideController();
        animOverrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
        animOverrideController["SecondsLeftTextShow"] = animationClip;
        anim.runtimeAnimatorController = animOverrideController;
    }*/

    public static NumberOfSecondsLeftTextEvents instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Animator levelSecondsLeftAnimator;

    // EVENT: fires at each second and a half (0.30, 1.30, 2.30, ...) (when the text has maximum alpha).
    public void LevelSecondsLeftChange()
    {
        if (LevelParameters.secondsLeft == 0)
            levelSecondsLeftAnimator.SetTrigger("TimeExpired");
    }
}
