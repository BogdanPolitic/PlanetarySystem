using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedRewardClaims : MonoBehaviour
{
    public enum NowInStackText
    {
        NOT_TRIGGERED,
        TRIGGERING_NEXT_CYCLE,
        TRIGGERING_THIS_CYCLE,
        STOPPED_TRIGGERING
    }

    public class PlanetReward
    {
        public GameObject container;
        public GameObject texture;
        public Text gainedPoints;
        public Text totalPoints;
        public Text nowInStackText;
        public NowInStackText nowInStackTextState;
        public float changeDelay;
        public int textChangeStartCount;
        public bool animatingGainedPoints;
        public bool animatingTotalPoints;

        public PlanetReward(GameObject container, GameObject texture, Text gainedPoints, Text totalPoints, Text nowInStackText, float changeDelay)
        {
            this.container = container;
            this.texture = texture;
            this.gainedPoints = gainedPoints;
            this.totalPoints = totalPoints;
            this.nowInStackText = nowInStackText;
            nowInStackTextState = NowInStackText.NOT_TRIGGERED;
            this.changeDelay = changeDelay;
            animatingGainedPoints = false;
            animatingTotalPoints = false;
        }
    }

    private static AnimatedRewardClaims instance;
    [SerializeField] Text rewardText;
    [SerializeField] Text templateRewardTextChange;
    List<PlanetReward> rewardsUIElements;
    private float syncDelay;
    private bool templateChangingStarted;
    private bool firstTemplateChanging;

    [SerializeField] Animator nowInStackTextChangeAnimator;
    [SerializeField] Text nowInStackTextTemplate;

    public static AnimatedRewardClaims GetInstance()
    {
        return instance;
    }

    public void TemplateChangingStarted()
    {
        templateChangingStarted = true;
        firstTemplateChanging = true;
        nowInStackTextChangeAnimator.SetTrigger("AllowTextChange");
    }

    // Called once the rewards are calculated (just before entering the reward claiming screen).
    public void UpdatePlanetRewards()
    {
        foreach (PlanetReward planetReward in rewardsUIElements)
        {

            if (!Inventory.inventory.ContainsKey(planetReward.container.name)
                || !Inventory.planetStackRewards.ContainsKey(planetReward.container.name))
            {
                // If it enters the if branch, the Inventory.inventory dictionary names and the GameObject names in the reward scene are NOT consistent. Please check them again.
                continue;
            }

            planetReward.totalPoints.text = Inventory.inventory[planetReward.container.name].quantity.ToString();
            planetReward.gainedPoints.text = "+" + Inventory.planetStackRewards[planetReward.container.name];
        }
    }

    // Called by the template text change event (changes are made about 3 times a second, so this is called roughly 3 times a second). At this point, the template text has scale ZERO.
    public void TextChanged()
    {
        foreach (PlanetReward planetReward in rewardsUIElements)
        {
            if (Inventory.planetStackRewards[planetReward.container.name] == 0)
            {
                planetReward.animatingGainedPoints = false;
                if (planetReward.nowInStackTextState == NowInStackText.NOT_TRIGGERED)
                    planetReward.nowInStackTextState = NowInStackText.TRIGGERING_NEXT_CYCLE;
                continue;
            }

            Inventory.inventory[planetReward.container.name].quantity++;
            Inventory.planetStackRewards[planetReward.container.name]--;
        }
        UpdatePlanetRewards();
    }

    IEnumerator TextChange(Text text, Vector3 setToSize, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.rectTransform.localScale = setToSize;
    }

    // Called at the middle of the interval of the text-change-animation.
    public void MiddleOfTheTextChange()
    {
        foreach (PlanetReward planetReward in rewardsUIElements)
        {
            if (templateChangingStarted && firstTemplateChanging)
            {
                planetReward.animatingGainedPoints = true;
                planetReward.animatingTotalPoints = true;
            }

            if (Inventory.planetStackRewards[planetReward.container.name] == 0)
                planetReward.animatingTotalPoints = false;
        }

        if (firstTemplateChanging)
            firstTemplateChanging = false;
    }

    // Called after the "Now in stack" text is at maximum opacity and it's going to restart to the minimum opacity.
    public void NowInStackTextChanged()
    {
        foreach (PlanetReward planetReward in rewardsUIElements)
        {
            if (planetReward.nowInStackTextState == NowInStackText.TRIGGERING_NEXT_CYCLE)
            {
                planetReward.nowInStackTextState = NowInStackText.TRIGGERING_THIS_CYCLE;
            } else if (planetReward.nowInStackTextState == NowInStackText.TRIGGERING_THIS_CYCLE)
            {
                planetReward.nowInStackTextState = NowInStackText.STOPPED_TRIGGERING;
            }
        }
    }

    private void Awake()
    {
        instance = this;

        syncDelay = 0.05f;
        float totalDelay = syncDelay;

        rewardsUIElements = new List<PlanetReward>();
        foreach (Transform container in transform)
        {
            if (container.tag != "RewardsText")
                rewardsUIElements.Add(
                    new PlanetReward(
                        container.gameObject,
                        container.transform.GetChild(0).gameObject,
                        container.transform.GetChild(1).GetComponent<Text>(),
                        container.transform.GetChild(2).GetComponent<Text>(),
                        container.transform.GetChild(3).GetComponent<Text>(),
                        totalDelay += syncDelay
                    )
                );
        }

        templateChangingStarted = false;
        firstTemplateChanging = false;
    }

    void Update()
    {
        foreach (PlanetReward planetReward in rewardsUIElements)
        {
            if (planetReward.animatingGainedPoints)
                StartCoroutine(TextChange(planetReward.gainedPoints, templateRewardTextChange.rectTransform.localScale, planetReward.changeDelay));
            if (planetReward.animatingTotalPoints)
                StartCoroutine(TextChange(planetReward.totalPoints, templateRewardTextChange.rectTransform.localScale, planetReward.changeDelay + syncDelay * 8));
            if (planetReward.nowInStackTextState == NowInStackText.TRIGGERING_THIS_CYCLE) {
                Color c = planetReward.nowInStackText.color;
                planetReward.nowInStackText.color = new Color(c.r, c.g, c.b, nowInStackTextTemplate.color.a);
            }
        }
    }
}
