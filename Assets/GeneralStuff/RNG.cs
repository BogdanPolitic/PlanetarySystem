using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG : MonoBehaviour
{
    static System.Random randomGen;
    static float randomNumber;

    public static int getRandomInt(int inf, int sup)
    {
        if (inf > sup)
            return 0;

        float randomFloat = 0;

        do
        {
            randomFloat = getRandomFloat(0, 10);
            randomFloat -= randomFloat % 1;
        }
        while (!(inf <= randomFloat && randomFloat <= sup));

        return (int) randomFloat;
    }

    public static float getRandomFloat(float inf, float sup)
    {
        if (inf > sup)
            return 0;

        randomNumber = Mathf.Sqrt(2) + Mathf.Sqrt(3) * randomNumber;
        randomNumber = randomNumber % 1;

        return inf + (sup - inf) * randomNumber;
    }

    public static Vector3 getRandomVector3(float inf, float sup)
    {
        if (inf > sup)
            return Vector3.zero;

        return new Vector3(
                        getRandomFloat(inf, sup),
                        getRandomFloat(inf, sup),
                        getRandomFloat(inf, sup)
                    );
    }

    public static void init()      // daca e nevoie la primul frame al scenei, acest modul RNG va trebui initializat prin aceasta functie, nu prin Start()
    {
        randomGen = new System.Random();
        randomNumber = randomGen.Next(0, 99);
    }

    private void Start()
    {
        init();
    }
}
