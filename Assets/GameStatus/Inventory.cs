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

    public static Dictionary<string, PlanetStack> inventory = null;
    public static int numberOfNonEmptyStacks;

    // error codes for local functions:
    public const int NO_SUCH_PLANET = -1;
    public const int STACK_ALREADY_EMPTY = -2;

    public static void initializeInventory()
    {
        if (inventory != null)
            return;

        inventory = new Dictionary<string, PlanetStack>();
        numberOfNonEmptyStacks = 0;

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
}
