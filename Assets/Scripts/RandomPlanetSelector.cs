using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlanetSelector : MonoBehaviour
{
    // alege un stack random din Inventory care are quantity > 0 (adica care are cel putin o planeta in stocul sau)
    public static Inventory.PlanetStack selectRandomPlanet() // de fiecare data cand se apeleaza asta, se returneaza planeta random selectata la select-ul precedent, si se genereaza un nou numar random pentru urmatoarea planeta ce va fi selectata
    {
        // generam un index random (indexul trebuie sa fie intre 1 si numarul de stackuri nenule din inventory)
        int randomStack = RNG.getRandomInt(1, Inventory.numberOfNonEmptyStacks);

        int i = 0;
        IEnumerator it = Inventory.inventory.Values.GetEnumerator();
        // iteram prin invetory pana gasim al randomStack-lea stack cu acest constraint (quantity > 0)
        while (it.MoveNext())
        {
            Inventory.PlanetStack stack = (Inventory.PlanetStack) it.Current;
            if (stack.quantity > 0)
                i++;

            if (i == randomStack)
            {
                return stack;
            }
        }

        return null;  // doar printr-o eroare se ajunge aici. Cel mai probabil din cauza ca Inventory.numberOfNonEmptyStacks nu e updatat in mod corespunzator
    }
}
