using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButtonListener : MonoBehaviour
{
    class Axis
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

    class Button
    {
        public int id;
        public Axis toggleAxis;
        public bool greaterCoord;
        public int intentionTo;
        public Button(int _id, Axis _toogleAxis, bool _greaterCoord, int _intentionTo) {
            id = _id;
            toggleAxis = _toogleAxis;
            greaterCoord = _greaterCoord;
            intentionTo = _intentionTo;
            buttonsMap[id] = this;
        }
    }

    class Intention
    {
        public int to;
        public int from;
        public Quaternion[] angles;
        public Intention(int from)
        {
            this.to = NOWHERE;
            this.from = from;
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

    public static bool locked = true;   // first locked
    RaycastHit hit;
    Ray ray;
    int buttonHash;
    string currentTag;
    GameObject quad;
    GameObject glowingText;
    static Camera currentCamera; // the one and only camera
    float init_r;
    float init_g;
    float init_b;

    const int NUMBER_OF_DIRECTIONS = 6; // numarul 6 nu are legatura cu valorile constantelor de mai jos, ci reprezinta numarul lor (exceptand NOWHERE)
    // urmatoarele constante reprezinta identificatori de destinatii (locuri) din meniu. Butoanele care au ca destinatie una din ele, vor purta fix acel identificator.
    const int NOWHERE = -1;
    const int MENU = 0;
    const int LOAD_GAME = 1;
    const int OPTIONS = 2;
    const int QUIT_GAME = 3;
    const int PROFILE = 4;
    const int GAME_SETTINGS = 5;

    // axele:
    const int CONST_NO_AXIS = -1;
    const int CONST_AXIS_X = 0;
    const int CONST_AXIS_Y = 1;
    const int CONST_AXIS_Z = 2;

    static Axis NO_AXIS = new Axis(CONST_NO_AXIS, -1.04f, -1f);
    static Axis AXIS_X = new Axis(CONST_AXIS_X, 0.104f, 0.1f);
    static Axis AXIS_Y = new Axis(CONST_AXIS_Y, 0.104f, 0.1f);
    static Axis AXIS_Z = new Axis(CONST_AXIS_Z, 0.104f, 0.1f);

    const int NUMBER_OF_BUTTONS = 7;
    // menu window
    static Button NEW_GAME_BUTTON;// = new Button(0, AXIS_Z, true, NOWHERE);
    static Button LOAD_GAME_BUTTON;// = new Button(1, AXIS_Z, true, LOAD_GAME);
    static Button OPTIONS_BUTTON;// = new Button(2, AXIS_Z, true, OPTIONS);
    static Button QUIT_GAME_BUTTON;// = new Button(3, AXIS_Z, true, QUIT_GAME);

    // quit_game window
    static Button QUIT_REQUEST_YES_BUTTON;// = new Button(4, AXIS_Y, false, NOWHERE);
    static Button QUIT_REQUEST_NO_BUTTON;// = new Button(5, AXIS_Y, false, MENU);

    // load_game window
    static Button LOAD_LEVEL_0_BUTTON;// = new Button(6, AXIS_Z, true, NOWHERE);

    static Button[] buttonsMap;

    static Intention intention;

    float rotationTime;
    float progress_unit;

    float progress;

    void SetTextIntensity(float intensity)
    {
        Color highlightedColor = glowingText.GetComponent<MeshRenderer>().material.GetColor("HighlightedColor");
        if (highlightedColor != null)
        {
            float factor = Mathf.Pow(2, intensity);
            highlightedColor.r = init_r * factor;
            highlightedColor.g = init_g * factor;
            highlightedColor.b = init_b * factor;
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

        if (buttonName == "Level_0")
        {
            return LOAD_LEVEL_0_BUTTON.id;
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
        if (progress != 1)
        {
            progress = Mathf.Clamp01(progress + progress_unit);
            currentCamera.transform.rotation = Quaternion.Slerp(intention.angles[intention.from], intention.angles[intention.to], progress);
        }
        else
        {
            //intention.from = intention.to; -- aici am stat sa debugguiesc O JUMATE DE ZI !!!!!!!!!!!!!!!!!!!!!
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

    void Start()
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
        LOAD_LEVEL_0_BUTTON = new Button(6, AXIS_X, true, NOWHERE);


        currentCamera = FindObjectOfType<Camera>();
        intention = new Intention(MENU);

        rotationTime = 0.25f; // cate secunde dureaza rotirea
        progress_unit = 1f / (rotationTime / Time.deltaTime);

        locked = false;
    }

    public bool BeforeFirstFrame(string currentButtonName)
    {
        if (buttonHash == getButtonHash(currentButtonName)) // ne aflam deja la obiectul potrivit (a fost asignat acelasi buttonHash frame-ul trecut)
            return true;
        buttonHash = getButtonHash(currentButtonName);
        currentTag = "ToRaycast_" + currentButtonName;
        quad = GameObject.Find(currentButtonName + "Box");
        glowingText = GameObject.Find(currentButtonName + "Text");

        Color highlightedColor = glowingText.GetComponent<MeshRenderer>().material.GetColor("HighlightedColor");
        if (highlightedColor != null)
        {
            init_r = highlightedColor.r;
            init_g = highlightedColor.g;
            init_b = highlightedColor.b;
        }
        RequestUpdate();
        return false;
    }

    public void RequestUpdate()
    {
        int buttonIntention = buttonsMap[buttonHash].intentionTo;

        if (intention.to != NOWHERE)
        {
            RotateToSelected();
        }
        intention.from = UpdateIntentionFrom();


        ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f) && hit.transform.tag == currentTag)
        {
            quad.transform.position = setPosition(quad.transform.position, buttonsMap[buttonHash].toggleAxis.coordWhenPressed, buttonsMap[buttonHash].toggleAxis.axis, buttonsMap[buttonHash].greaterCoord);
            SetTextIntensity(1.3f);

            if (Input.GetMouseButtonDown(0))
            {
                if (buttonHash == NEW_GAME_BUTTON.id)
                {
                    SceneManager.LoadScene(1);
                }
                else
                {
                    if (intention.to != buttonIntention)
                    {
                        progress = 0;
                        intention.to = buttonIntention;
                    }
                }
            }
        }
        else
        {
            quad.transform.position = setPosition(quad.transform.position, buttonsMap[buttonHash].toggleAxis.coordWhenReleased, buttonsMap[buttonHash].toggleAxis.axis, buttonsMap[buttonHash].greaterCoord);
            SetTextIntensity(0.3f);
        }
    }
}
