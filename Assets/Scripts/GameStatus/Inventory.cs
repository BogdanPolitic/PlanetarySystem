using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public class PlanetStack
    {
        public string name;
        public string pathToImage;
        public string pathToTexture;   // dunno if I'll use this in the future (instead of the hardcodes in the PathReader1.cs)
        public int quantity;
        public float size;
        public float sizeDiscountOnNext;
        public int numberOfABDSTokens;

        public float representationScale;

        public PlanetStack(string name, string pathToImage, string pathToTexture, int quantity, float size, float sizeDiscountOnNext, int numberOfABDSTokens, float representationScale)
        {
            this.name = name;
            this.pathToImage = pathToImage;
            this.pathToTexture = pathToTexture;
            this.quantity = quantity;
            this.size = size;
            this.sizeDiscountOnNext = sizeDiscountOnNext;
            this.numberOfABDSTokens = numberOfABDSTokens;
            this.representationScale = representationScale;
        }
    }

    /*public class LevelNumberOfPlanetsProgress
    {
        public int initial;
        public int final;

        public LevelNumberOfPlanetsProgress(int initial, int final)
        {
            this.initial = initial;
            this.final = final;
        }
    }*/


    public static Dictionary<string, PlanetStack> inventory = null;
    public static int numberOfNonEmptyStacks;

    private static Dictionary<string, int> levelInventoryAccounting = null;
    public static Dictionary<string, int> planetStackRewards = null;

    // error codes for local functions:
    public const int NO_SUCH_PLANET = -1;
    public const int STACK_ALREADY_EMPTY = -2;

    public static void initializeInventory()
    {
        if (inventory != null)
            return;

        inventory = new Dictionary<string, PlanetStack>();
        numberOfNonEmptyStacks = 0;

        levelInventoryAccounting = new Dictionary<string, int>();
        planetStackRewards = new Dictionary<string, int>();

        inventory["earth"] = new PlanetStack("earth",
                                            "PlanetSurfaces/3D_Projections/earth_spheric",
                                            "",
                                            5,
                                            12.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["jupiter"] = new PlanetStack("jupiter",
                                            "PlanetSurfaces/3D_Projections/jupiter_spheric",
                                            "",
                                            5,
                                            40.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["mars"] = new PlanetStack("mars",
                                            "PlanetSurfaces/3D_Projections/mars_spheric",
                                            "",
                                            5,
                                            8.0f,
                                            0.0f,
                                            0,
                                            1.0f);
        numberOfNonEmptyStacks++;

        inventory["mercury"] = new PlanetStack("mercury",
                                            "PlanetSurfaces/3D_Projections/mercury_spheric",
                                            "",
                                            5,
                                            14.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["neptune"] = new PlanetStack("neptune",
                                            "PlanetSurfaces/3D_Projections/neptune_spheric",
                                            "",
                                            5,
                                            18.0f,
                                            0.0f,
                                            0,
                                            1.0f);
        numberOfNonEmptyStacks++;

        inventory["pluto"] = new PlanetStack("pluto",
                                            "PlanetSurfaces/3D_Projections/pluto_spheric",
                                            "",
                                            5,
                                            4.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["saturn"] = new PlanetStack("saturn",
                                            "PlanetSurfaces/3D_Projections/saturn_spheric",
                                            "",
                                            5,
                                            35.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["uranus"] = new PlanetStack("uranus",
                                            "PlanetSurfaces/3D_Projections/uranus_spheric",
                                            "",
                                            5,
                                            10.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;

        inventory["venus"] = new PlanetStack("venus",
                                            "PlanetSurfaces/3D_Projections/venus_spheric",
                                            "",
                                            5,
                                            8.0f,
                                            0.0f,
                                            0,
                                            1.5f);
        numberOfNonEmptyStacks++;
    }

    public static int takeItemOfType(string planetName)
    {
        if (inventory[planetName] == null)
            return NO_SUCH_PLANET;

        if (inventory[planetName].quantity == 0)
            return STACK_ALREADY_EMPTY;

        inventory[planetName].quantity--;
        if (inventory[planetName].quantity == 0)
            numberOfNonEmptyStacks--;

        // + CONSUME EVENTUAL BONUS !!

        return 0;
    }

    public static int addItemOfType(string planetName)
    {
        if (inventory[planetName] == null)
            return NO_SUCH_PLANET;

        inventory[planetName].quantity++;
        return 0;
    }



    // For each planet that ended up in equilibrium, it's a plus (+) for that PlanetStack. For each planet that ended up in collision, it's a minus (-) for that PlanetStack.
    // (+)es and (-)es can even out.
    // To be called at the beginning of each level.
    public static void BeginLevelAccounting()
    {
        foreach (string planetStr in inventory.Keys)
        {
            levelInventoryAccounting[planetStr] = 0;
            planetStackRewards[planetStr] = 0;
        }
    }

    public static void PlanetConsumed(string stackName)
    {
        levelInventoryAccounting[stackName] += 1;
    }

    public static void PlanetConsumedIntoFailure(string stackName)
    {
        levelInventoryAccounting[stackName] -= 2;
    }

    // To be called at the end (upon the completion) of each level.
    public static void CalculateLevelRewards()
    {
        int sumOfStackProgress = 0;
        int numberOfProgressStacks = 0;

        foreach (int stackProgress in levelInventoryAccounting.Values)
        {
            if (stackProgress > 0)
            {
                sumOfStackProgress += stackProgress;
                numberOfProgressStacks++;
            }
        }

        if (sumOfStackProgress == 0)    // Poor gameplay. No rewards for this level! The reward claiming screen will still show, but 0 planets of each stack will be given.
            return;

        int numberOfPlanetsAsReward = LevelParameters.totalPoints / ValueSheet.totalPlanetsRewardDiv;
        for (int rewardPlanetIdx = 0; rewardPlanetIdx < numberOfPlanetsAsReward; rewardPlanetIdx++)
        {
            planetStackRewards[ChooseRandomStackByProbability(sumOfStackProgress)]++;
        }

        AnimatedRewardClaims.GetInstance().UpdatePlanetRewards();
    }

    private static string ChooseRandomStackByProbability(int sumStacks)
    {
        int randomNumber = RNG.getRandomInt(1, sumStacks);
        int iterationSum = 0;

        foreach (string stackProgress in levelInventoryAccounting.Keys)
        {
            if (levelInventoryAccounting[stackProgress] <= 0)
                continue;

            if (randomNumber >= iterationSum + 1 && randomNumber <= iterationSum + levelInventoryAccounting[stackProgress])
            {
                return stackProgress;
            }
            iterationSum += levelInventoryAccounting[stackProgress];
        }

        return null;    // Should never be reached. If it does, either this function or the CalculateLevelRewards() has a bug.
    }
}
