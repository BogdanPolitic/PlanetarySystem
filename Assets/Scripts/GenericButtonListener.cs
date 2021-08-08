using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericButtonListener : MonoBehaviour
{
    public class Axis
    {
        public int axis;
        public float coordWhenPressed;
        public float coordWhenReleased;
        public Axis(int _axis, float _coordWhenPressed, float _coordWhenReleased)
        {
            axis = _axis;
            coordWhenPressed = _coordWhenPressed;
            coordWhenReleased = _coordWhenReleased;
        }
    }

    public class Button
    {
        public int id;
        public Axis toggleAxis;
        public bool greaterCoord;
        public int intentionTo;
        public Button(int _id, Axis _toogleAxis, bool _greaterCoord, int _intentionTo)
        {
            id = _id;
            toggleAxis = _toogleAxis;
            greaterCoord = _greaterCoord;
            intentionTo = _intentionTo;
            buttonsMap[id] = this;
        }
    }

    class Intention
    {
        public int subject;
        public int to;
        public int from;
        public float progress;
        public static float progressUnit;
        public Quaternion[] angles;
        public Intention(int from)
        {
            this.subject = -1;  // noone first
            this.to = NOWHERE;
            this.from = from;
            this.progress = 0;
            angles = new Quaternion[NUMBER_OF_DIRECTIONS];

            Quaternion initialCamRot = currentCamera.transform.rotation;
            angles[MENU] = initialCamRot;
            angles[LOAD_GAME] = returnAngle(initialCamRot, new Vector3(0, 90, 0));
            angles[OPTIONS] = returnAngle(initialCamRot, new Vector3(-90, 0, 0));
            angles[QUIT_GAME] = returnAngle(initialCamRot, new Vector3(90, 0, 0));
            angles[PROFILE] = returnAngle(initialCamRot, new Vector3(180, 0, 0));
            angles[GAME_SETTINGS] = returnAngle(initialCamRot, new Vector3(0, -90, 0));
        }
    }

    public class ObjectCharacteristics
    {
        public int buttonHash;
        public string currentTag;
        public GameObject box;
        public GameObject glowingText;
        public bool availableLevelOrNoLevelButton; // false - if it's an UNavailable level; true - if it's an available level OR a button that does NOT represent a level (f.e. the "New Game" button).
        // In short, buttons with (availableLevelOrNoLevelButton == true) should have red text, and the other ones should have black text.

        public ObjectCharacteristics(int _buttonHash, string _currentTag, GameObject _box, GameObject _glowingText)
        {
            buttonHash = _buttonHash;
            currentTag = _currentTag;
            box = _box;
            glowingText = _glowingText;

            availableLevelOrNoLevelButton = 
                (_buttonHash < level0Index || _buttonHash >= level0Index + numberOfLevels)  // It's not a level load button.
                || (_buttonHash - level0Index <= LevelParameters.maxReachedLevel + 1);      // It's a level load button AND it's an available level to choose.
            // LevelParameters.maxReachedLevel + 1 because you can access the next level to the highest one that you succeeded.
        }
    }

    public static bool locked = true;   // first locked
    static int buttonHash = -1; // este extrem de importanta setarea la -1! Altfel by default ar fi setata la zero, care este deja hash-ul butonului de NewGame. Deci vom avea erori din cauza parasirii functiei BeforeFirstFrame()
    string currentTag;
    GameObject quad;
    GameObject glowingText;
    static Camera currentCamera; // the one and only camera
    Color availableButtonTextColor;
    Color unavailableButtonTextColor;

    const int NUMBER_OF_DIRECTIONS = 6; // numarul 6 nu are legatura cu valorile constantelor de mai jos, ci reprezinta numarul lor (exceptand NOWHERE)
    // urmatoarele constante reprezinta identificatori de destinatii (locuri) din meniu. Butoanele care au ca destinatie una din ele, vor purta fix acel identificator.
    public static int NOWHERE = -1;
    public static int MENU = 0;
    public static int LOAD_GAME = 1;
    public static int OPTIONS = 2;
    public static int QUIT_GAME = 3;
    public static int PROFILE = 4;
    public static int GAME_SETTINGS = 5;

    // axele:
    public const int CONST_NO_AXIS = -1;
    public const int CONST_AXIS_X = 0;
    public const int CONST_AXIS_Y = 1;
    public const int CONST_AXIS_Z = 2;

    static Axis NO_AXIS = new Axis(CONST_NO_AXIS, -1.04f, -1f);
    static Axis AXIS_X = new Axis(CONST_AXIS_X, 0.104f, 0.1f);
    static Axis AXIS_Y = new Axis(CONST_AXIS_Y, 0.104f, 0.1f);
    static Axis AXIS_Z = new Axis(CONST_AXIS_Z, 0.104f, 0.1f);

    const int NUMBER_OF_BUTTONS = 16;
    // menu window
    public static Button NEW_GAME_BUTTON;// = new Button(0, AXIS_Z, true, NOWHERE);
    public static Button LOAD_GAME_BUTTON;// = new Button(1, AXIS_Z, true, LOAD_GAME);
    public static Button OPTIONS_BUTTON;// = new Button(2, AXIS_Z, true, OPTIONS);
    public static Button QUIT_GAME_BUTTON;// = new Button(3, AXIS_Z, true, QUIT_GAME);

    // quit_game window
    public static Button QUIT_REQUEST_YES_BUTTON;// = new Button(4, AXIS_Y, false, NOWHERE);
    public static Button QUIT_REQUEST_NO_BUTTON;// = new Button(5, AXIS_Y, false, MENU);

    // load_game window
    public static Button BACK_TO_MENU_BUTTON;// = new Button(6, AXIS_Z, true, MENU);
    public static Button LOAD_LEVEL_0_BUTTON;// = new Button(7, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_1_BUTTON;// = new Button(8, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_2_BUTTON;// = new Button(9, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_3_BUTTON;// = new Button(10, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_4_BUTTON;// = new Button(11, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_5_BUTTON;// = new Button(12, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_6_BUTTON;// = new Button(13, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_7_BUTTON;// = new Button(14, AXIS_Z, true, NOWHERE);
    public static Button LOAD_LEVEL_8_BUTTON;// = new Button(15, AXIS_Z, true, NOWHERE);

    static int level0Index = 7;
    public static int numberOfLevels = 9;

    public static Button[] buttonsMap;

    static Intention intention;

    static float rotationTime;

    // For color initializing purposes only.
    [SerializeField] Material buttonTextMaterial;

    void SetTextIntensity(GameObject glowingText, float intensity, bool availableButtonText)
    {
        Color initialColor = availableButtonText
            ? availableButtonTextColor
            : unavailableButtonTextColor;
        Color highlightedColor = glowingText.GetComponent<MeshRenderer>().material.GetColor("HighlightedColor");
        if (highlightedColor != null)
        {
            float factor = Mathf.Pow(2, intensity);
            highlightedColor.r = initialColor.r * factor;
            highlightedColor.g = initialColor.g * factor;
            highlightedColor.b = initialColor.b * factor;
            glowingText.GetComponent<MeshRenderer>().material.SetColor("HighlightedColor", highlightedColor);
        }
    }

    static int getButtonHash(string buttonName)   // nu sunt sigur de functia asta. Atunci cand suntem in alta fereastra si apasam butonul de back, oare putem generaliza asta? hmm... posibil dar inca nu m-am gandit cum
    {
        // butoanele ce indica directii
        if (buttonName == "LoadGame")
            return LOAD_GAME;
        if (buttonName == "QuitGame")
            return QUIT_GAME;
        if (buttonName == "Options")
            return OPTIONS;

        // alte butoane
        if (buttonName == "NewGame")
            return NEW_GAME_BUTTON.id;
        if (buttonName == "QuitRequestYes")
            return QUIT_REQUEST_YES_BUTTON.id;
        if (buttonName == "QuitRequestNo")
            return QUIT_REQUEST_NO_BUTTON.id;

        if (buttonName == "BackToMenu")
        {
            return BACK_TO_MENU_BUTTON.id;
        }
        if (buttonName == "Level_0")
        {
            return LOAD_LEVEL_0_BUTTON.id;
        }
        if (buttonName == "Level_1")
        {
            return LOAD_LEVEL_1_BUTTON.id;
        }
        if (buttonName == "Level_2")
        {
            return LOAD_LEVEL_2_BUTTON.id;
        }
        if (buttonName == "Level_3")
        {
            return LOAD_LEVEL_3_BUTTON.id;
        }
        if (buttonName == "Level_4")
        {
            return LOAD_LEVEL_4_BUTTON.id;
        }
        if (buttonName == "Level_5")
        {
            return LOAD_LEVEL_5_BUTTON.id;
        }
        if (buttonName == "Level_6")
        {
            return LOAD_LEVEL_6_BUTTON.id;
        }
        if (buttonName == "Level_7")
        {
            return LOAD_LEVEL_7_BUTTON.id;
        }
        if (buttonName == "Level_8")
        {
            return LOAD_LEVEL_8_BUTTON.id;
        }

        return NOWHERE;
    }

    static Quaternion returnAngle(Quaternion initialRotation, Vector3 rotate)
    {
        GameObject dummy = new GameObject();
        dummy.transform.rotation = initialRotation;
        dummy.transform.Rotate(rotate);
        Quaternion finalPosition = dummy.transform.rotation;
        Destroy(dummy);

        return finalPosition;
    }

    void RotateToSelected()
    {
        if (intention.progress != 1)
        {
            intention.progress = Mathf.Clamp01(intention.progress + Intention.progressUnit);
            currentCamera.transform.rotation = Quaternion.Slerp(intention.angles[intention.from], intention.angles[intention.to], intention.progress);
        }
        else
        {
            intention.to = NOWHERE;
        }
    }

    Vector3 setPosition(Vector3 position, float value, int axis, bool isGreaterCoord)
    {
        if (isGreaterCoord)
            value = 1f - value;

        switch (axis)
        {
            case CONST_AXIS_X:
                position.x = value;
                break;
            case CONST_AXIS_Y:
                position.y = value;
                break;
            case CONST_AXIS_Z:
                position.z = value;
                break;
            default:
                break;
        }
        return position;
    }

    int UpdateIntentionFrom()
    {
        Quaternion cameraRotation = currentCamera.transform.rotation;
        for (int idx = 0; idx < intention.angles.Length; idx++)
        {
            if (cameraRotation == intention.angles[idx])
            {
                return idx;
            }
        }
        return intention.from;
    }

    private void Awake()
    {
        availableButtonTextColor = new Color(
            buttonTextMaterial.GetColor("HighlightedColor").r,
            buttonTextMaterial.GetColor("HighlightedColor").g,
            buttonTextMaterial.GetColor("HighlightedColor").b
        );
        unavailableButtonTextColor = ValueSheet.unavailableLevelButtonTextColor;
    }

    public static void InitializeButtons()              // in mod normal asta era metoda Start(), dar ne trebuie initializarea lui buttonsMap in metoda Start() din ScrollMechanics
    {
        buttonsMap = new Button[NUMBER_OF_BUTTONS];
        // menu window
        NEW_GAME_BUTTON = new Button(0, AXIS_Z, true, NOWHERE);
        LOAD_GAME_BUTTON = new Button(1, AXIS_Z, true, LOAD_GAME);
        OPTIONS_BUTTON = new Button(2, AXIS_Z, true, OPTIONS);
        QUIT_GAME_BUTTON = new Button(3, AXIS_Z, true, QUIT_GAME);

        // quit_game window
        QUIT_REQUEST_YES_BUTTON = new Button(4, AXIS_Y, false, NOWHERE);
        QUIT_REQUEST_NO_BUTTON = new Button(5, AXIS_Y, false, MENU);

        // load_game window
        BACK_TO_MENU_BUTTON = new Button(6, AXIS_X, true, MENU);
        LOAD_LEVEL_0_BUTTON = new Button(7, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_1_BUTTON = new Button(8, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_2_BUTTON = new Button(9, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_3_BUTTON = new Button(10, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_4_BUTTON = new Button(11, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_5_BUTTON = new Button(12, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_6_BUTTON = new Button(13, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_7_BUTTON = new Button(14, AXIS_X, true, NOWHERE);
        LOAD_LEVEL_8_BUTTON = new Button(15, AXIS_X, true, NOWHERE);

        currentCamera = FindObjectOfType<Camera>();
        //intention = new Intention(MENU);
        intention = new Intention(LevelParameters.returnMenuPath);

        if (LevelParameters.returnMenuPath == LOAD_GAME)
        {
            currentCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        rotationTime = 0.25f; // cate secunde dureaza rotirea
        Intention.progressUnit = 1f / (rotationTime / Time.deltaTime);

        locked = false;

        LevelSpecification.Initialize();
    }

    public ObjectCharacteristics GetButtonCharacteristics(string currentButtonName)
    {
        buttonHash = getButtonHash(currentButtonName);
        currentTag = "ToRaycast_" + currentButtonName;
        quad = GameObject.Find(currentButtonName + "Box");
        glowingText = GameObject.Find(currentButtonName + "Text");

        return new ObjectCharacteristics(buttonHash, currentTag, quad, glowingText);
    }

    public void RequestUpdate(int buttonHash, string currentTag, GameObject quad, GameObject glowingText, bool availableLevelOrNoLevelButton)
    {
        int buttonIntention = buttonsMap[buttonHash].intentionTo;
        
        if (intention.to != NOWHERE && intention.subject == buttonHash) // daca (pe un touchScreen) se apasa pe mai multe butoane deodata, subiectul intentiei este butonul considerat ultimul apasat (dupa ordinea pe care o face sistemul, adica ultima asignare de atribuire a intention.subject)
        {
            RotateToSelected();
        }
        intention.from = UpdateIntentionFrom();

        RaycastHit hit;
        Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f) && hit.transform.tag == currentTag)
        {
            quad.transform.position = setPosition(quad.transform.position, buttonsMap[buttonHash].toggleAxis.coordWhenPressed, buttonsMap[buttonHash].toggleAxis.axis, buttonsMap[buttonHash].greaterCoord);
            SetTextIntensity(glowingText, 1.3f, availableLevelOrNoLevelButton);

            if (Input.GetMouseButtonDown(0))
            {
                if (buttonHash == NEW_GAME_BUTTON.id)
                {
                    LevelSpecification.changeToLevel(0);    // new game => level 0
                    FadeManagement.GetInstance().MenuProceedToFadeOut();
                }
                else if (buttonHash >= level0Index && buttonHash < level0Index + numberOfLevels)
                {
                    int buttonLevel = buttonHash - level0Index;
                    if (buttonLevel <= LevelParameters.maxReachedLevel + 1)
                    {
                        LevelSpecification.changeToLevel(buttonLevel);
                        FadeManagement.GetInstance().MenuProceedToFadeOut();
                    }
                }
                else
                {
                    if (intention.to != buttonIntention)
                    {
                        intention.subject = buttonHash;
                        intention.progress = 0;
                        intention.to = buttonIntention;
                    }
                }
            }
        }
        else
        {
            quad.transform.position = setPosition(quad.transform.position, buttonsMap[buttonHash].toggleAxis.coordWhenReleased, buttonsMap[buttonHash].toggleAxis.axis, buttonsMap[buttonHash].greaterCoord);
            SetTextIntensity(glowingText, 0.3f, availableLevelOrNoLevelButton);
        }
    }
}
