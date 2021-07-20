using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetIndex : MonoBehaviour
{
    static string[] planetNames = new string[]
    {
        "Earth",
        "Jupiter",
        "Mars",
        "Mercury",
        "Neptune",
        "Pluto",
        "Saturn",
        "Uranus",
        "Venus"
    };

    public static string getPlanetName(int index)
    {
        return planetNames[index];
    }

    public static Inventory.PlanetStack getPlanetStack(int index)
    {
        return Inventory.inventory[getPlanetName(index)];
    }
}
