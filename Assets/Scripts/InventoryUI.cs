using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    public Texture backToMenuTexture;

    public Texture currentPlanetTexture;

    public Texture earthTexture;
    public Texture jupiterTexture;
    public Texture marsTexture;
    public Texture mercuryTexture;
    public Texture neptuneTexture;
    public Texture saturnTexture;
    public Texture uranusTexture;
    public Texture venusTexture;
    public Texture plutoTexture;

    const int OUTSIDE_INVENTORY = 0;
    const int ENTERING_INVETORY = 1;
    const int INSIDE_INVENTORY = 2;
    const int EXITING_INVENTORY = 3;

    Vector3 lastPositionOutsideInventory;
    Quaternion lastRotationOutsideInventory;

    int routeState;

    float screenWidth;
    float screenHeight;
    void Start()
    {
        routeState = OUTSIDE_INVENTORY;
        lastPositionOutsideInventory = Vector3.zero;
        lastRotationOutsideInventory = Quaternion.identity;

        earthTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/earth_spheric");
        jupiterTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/jupiter_spheric");
        marsTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/mars_spheric");
        mercuryTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/mercury_spheric");
        neptuneTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/neptune_spheric");
        saturnTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/saturn_spheric");
        uranusTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/uranus_spheric");
        venusTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/earth_spheric");
        plutoTexture = (Texture)Resources.Load("PlanetSurfaces/3D_Projections/pluto_spheric");
    }

    private void Update()
    {
        if (MainSceneUI.IsIntoInventory())
        {
            if (routeState == ENTERING_INVETORY)
                routeState = INSIDE_INVENTORY;
            if (routeState == OUTSIDE_INVENTORY)
                routeState = ENTERING_INVETORY;
        } else {
            if (routeState == EXITING_INVENTORY)
                routeState = OUTSIDE_INVENTORY;
            if (routeState == INSIDE_INVENTORY)
                routeState = EXITING_INVENTORY;
        }

        if (routeState == OUTSIDE_INVENTORY)
        {
            lastPositionOutsideInventory = FindObjectOfType<Camera>().gameObject.transform.position;
            lastRotationOutsideInventory = FindObjectOfType<Camera>().gameObject.transform.rotation;
        }

        if (routeState == ENTERING_INVETORY) { 
            FindObjectOfType<Camera>().gameObject.transform.position = new Vector3(0, -300, 0);
            FindObjectOfType<Camera>().gameObject.transform.rotation = new Quaternion(1, 0, 0, 1);
        }

        if (routeState == EXITING_INVENTORY)
        {
            MainSceneUI.ExitInventory();
            MainSceneUI.SetSceneMode(MainSceneUI.SceneMode.GAMEPLAY);
            FindObjectOfType<Camera>().gameObject.transform.position = lastPositionOutsideInventory;
            FindObjectOfType<Camera>().gameObject.transform.rotation = lastRotationOutsideInventory;
        }

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        //currentPlanetTexture = (Texture)Resources.Load(SceneParameters.currentPlanet.pathToImage);
    }
    
    private void OnGUI()
    {
        if (!MainSceneUI.IsIntoInventory())
            return;

        currentPlanetTexture = (Texture)Resources.Load(LevelParameters.currentPlanet.pathToImage);

        //GUI.DrawTexture(new Rect(10, 10, 150, 150), aTexture, ScaleMode.ScaleToFit, true, 1.0F);

        Rect currentPlanetRect = new Rect(screenWidth * 0.05f, screenHeight * 0.025f, screenWidth * 0.9f, screenHeight * 0.2f);
        GUI.Window(2, currentPlanetRect, currentPlanetWindow, "Current planet : " + LevelParameters.currentPlanet.name);

        float planetSectionWidth = screenWidth * 0.3f;
        float planetSectionHeight = screenHeight * 0.25f;


        Rect earthRect = new Rect(screenWidth * 0.05f, screenHeight * 0.225f, planetSectionWidth, planetSectionHeight);
        GUI.Window(3, earthRect, earthWindow, "EARTH");

        Rect jupiterRect = new Rect(screenWidth * 0.05f, screenHeight * 0.475f, planetSectionWidth, planetSectionHeight);
        GUI.Window(4, jupiterRect, jupiterWindow, "JUPITER");

        Rect marsRect = new Rect(screenWidth * 0.05f, screenHeight * 0.725f, planetSectionWidth, planetSectionHeight);
        GUI.Window(5, marsRect, marsWindow, "MARS");


        Rect mercuryRect = new Rect(screenWidth * 0.35f, screenHeight * 0.225f, planetSectionWidth, planetSectionHeight);
        GUI.Window(6, mercuryRect, mercuryWindow, "MERCURY");

        Rect neptuneRect = new Rect(screenWidth * 0.35f, screenHeight * 0.475f, planetSectionWidth, planetSectionHeight);
        GUI.Window(7, neptuneRect, neptuneWindow, "NEPTUNE");

        Rect saturnRect = new Rect(screenWidth * 0.35f, screenHeight * 0.725f, planetSectionWidth, planetSectionHeight);
        GUI.Window(8, saturnRect, saturnWindow, "SATURN");


        Rect uranusRect = new Rect(screenWidth * 0.65f, screenHeight * 0.225f, planetSectionWidth, planetSectionHeight);
        GUI.Window(9, uranusRect, uranusWindow, "URANUS");

        Rect venusRect = new Rect(screenWidth * 0.65f, screenHeight * 0.475f, planetSectionWidth, planetSectionHeight);
        GUI.Window(10, venusRect, venusWindow, "VENUS");

        Rect plutoRect = new Rect(screenWidth * 0.65f, screenHeight * 0.725f, planetSectionWidth, planetSectionHeight);
        GUI.Window(11, plutoRect, plutoWindow, "PLUTO");
    }

    void currentPlanetWindow(int formId)
    {
        GUILayout.BeginHorizontal();

            if (GUI.Button(new Rect(screenWidth * 0.04f, screenHeight * 0.05f, screenWidth * 0.12f, screenHeight * 0.12f), backToMenuTexture))
            {
                routeState = EXITING_INVENTORY;
            }

            GUI.DrawTexture(new Rect(screenWidth * 0.375f, screenHeight * 0.04f, screenWidth * 0.15f, screenHeight * 0.15f), currentPlanetTexture, ScaleMode.ScaleToFit, true, 1.5F);
            GUI.Button(new Rect(screenWidth * 0.75f, screenHeight * 0.06f, screenWidth * 0.1f, screenHeight * 0.1f), "Switch tokens: " + LevelParameters.numberOfSwitchTokens);

        GUILayout.EndHorizontal();
    }

    void earthWindow(int formId)
    {
        planetWindow(formId, earthTexture, Inventory.inventory["earth"]);
    }

    void jupiterWindow(int formId)
    {
        planetWindow(formId, jupiterTexture, Inventory.inventory["jupiter"]);
    }

    void marsWindow(int formId)
    {
        planetWindow(formId, marsTexture, Inventory.inventory["mars"]);
    }

    void mercuryWindow(int formId)
    {
        planetWindow(formId, mercuryTexture, Inventory.inventory["mercury"]);
    }

    void neptuneWindow(int formId)
    {
        planetWindow(formId, neptuneTexture, Inventory.inventory["neptune"]);
    }

    void saturnWindow(int formId)
    {
        planetWindow(formId, saturnTexture, Inventory.inventory["saturn"]);
    }

    void uranusWindow(int formId)
    {
        planetWindow(formId, uranusTexture, Inventory.inventory["uranus"]);
    }

    void venusWindow(int formId)
    {
        planetWindow(formId, venusTexture, Inventory.inventory["venus"]);
    }

    void plutoWindow(int formId)
    {
        planetWindow(formId, plutoTexture, Inventory.inventory["pluto"]);
    }

    void planetWindow(int formId, Texture planetTexture, Inventory.PlanetStack item)
    {
        GUILayout.BeginHorizontal();
        {
            GUI.DrawTexture(new Rect(screenWidth * -0.025f, screenHeight * 0.05f, screenWidth * 0.15f, screenHeight * 0.15f), planetTexture, ScaleMode.ScaleToFit, true, item.representationScale);

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUI.Label(new Rect(screenWidth * 0.1f, screenHeight * 0.05f, screenWidth * 0.075f, screenHeight * 0.05f), "Quantity: " + item.quantity);
                    if (GUI.Button(new Rect(screenWidth * 0.2f, screenHeight * 0.05f, screenWidth * 0.075f, screenHeight * 0.1f), "Switch"))
                    {
                        if (LevelParameters.numberOfSwitchTokens > 0)
                        {
                            LevelParameters.currentPlanet = item;
                            LevelParameters.numberOfSwitchTokens--;
                            PathReader1.cursorPlanetTexAssigned = false;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUI.Label(new Rect(screenWidth * 0.1f, screenHeight * 0.1f, screenWidth * 0.075f, screenHeight * 0.05f), "Size: " + item.size);
                GUI.Label(new Rect(screenWidth * 0.1f, screenHeight * 0.15f, screenWidth * 0.2f, screenHeight * 0.1f), "Bonus: 40% size discount on next");
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
}
