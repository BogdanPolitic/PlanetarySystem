using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMechanics : MonoBehaviour
{
    class ButtonObject
    {
        public GameObject parent;
        public GameObject box;
        public GameObject text;
        public ButtonObject(GameObject _parent, GameObject _box, GameObject _text)
        {
            parent = _parent;
            box = _box;
            text = _text;
        }
    }

    public class Materials
    {
        public Material defaultMaterial;
        public Material topDropdownMaterial;
        public Material bottomDropdownMaterial;
    }

    class PBRNoiseOffsetY
    {
        public float start;
        public float end;
        public PBRNoiseOffsetY(float _start, float _end)
        {
            start = _start;
            end = _end;
        }
    }

    private static ScrollMechanics instance;

    GameObject box;
    GameObject scrollCursor;
    GenericButtonListener.Button button;
    RaycastHit hit;
    Ray ray;

    const float MIN_Y = 0.38f;
    const float MAX_Y = 0.62f;
    const int NUMBER_OF_LEVELS = 9;
    const int WINDOW_SIZE = 5;  // showing max 5 levels into the current window. So you have to scroll up/down for searching the other ones.
    const float HIGHEST_Y = -0.05f; // TO BE REMOVED when upgrading
    const float Y_STEP = 0.06f;    // this should stay even after upgrade

    const int OFFSET_CUT_BOTTOM = -1;
    const int OFFSET_NOT_FADED = 0;
    const int OFFSET_CUT_TOP = 1;

    Materials boxMaterials;
    public Materials[] textMaterials;

    PBRNoiseOffsetY TOP_CUT_BOX = new PBRNoiseOffsetY(0.5f, 1.0f);
    PBRNoiseOffsetY BOTTOM_CUT_BOX = new PBRNoiseOffsetY(0.0f, 0.5f);

    PBRNoiseOffsetY TOP_CUT_TEXT = new PBRNoiseOffsetY(0.5f, 1.0f);
    PBRNoiseOffsetY BOTTOM_CUT_TEXT = new PBRNoiseOffsetY(0.0f, 0.5f);

    ButtonObject[] levels;

    static int firstCompletelyVisibleLevel = -1;
    static int lastCompletelyVisibleLevel = WINDOW_SIZE - 1;
    float levelsHiddenAbove = 0;
    float levelsHiddenBelow = 0;
    float unquantifyableOffset;

    Vector3 VectorsSum(Vector3 one, Vector3 two)
    {
        return new Vector3(
            one.x + two.x,
            one.y + two.y,
            one.z + two.z
        );
    }

    Vector3 SetYPosition(Vector3 position, float y)
    {
        return new Vector3(position.x, y, position.z);
    }

    Vector3 PositionWithOffset(GenericButtonListener.Axis axis, bool greaterCoord)
    {
        Vector3 currentBoxPosition = box.transform.position;
        Vector3 offset;
        float offsetOnCoord = 0.01f;
        switch (axis.axis)
        {
            case GenericButtonListener.CONST_AXIS_X:
                offset = new Vector3(offsetOnCoord, 0, 0);
                break;
            case GenericButtonListener.CONST_AXIS_Y:
                offset = new Vector3(0, offsetOnCoord, 0);
                break;
            case GenericButtonListener.CONST_AXIS_Z:
                offset = new Vector3(0, 0, offsetOnCoord);
                break;
            default:
                offset = Vector3.zero;
                break;
        }
        offset = offset * (greaterCoord ? -1 : 1);

        return VectorsSum(currentBoxPosition, offset);
    }

    Vector3 CalculateCurrentPosition(Vector3 hitPoint, Vector3 currentPosition, GenericButtonListener.Axis axis)    // stabileste cele doua coordonate care sa copieze hit.point-ul, si implicit cea constanta (care este cea a axei CONST_AXIS_ pe care se afla)
    {
        Vector3 position;
        switch (axis.axis)
        {
            case GenericButtonListener.CONST_AXIS_X:
                position = new Vector3(currentPosition.x, hitPoint.y, hitPoint.z);
                break;
            case GenericButtonListener.CONST_AXIS_Y:
                position = new Vector3(hitPoint.x, currentPosition.y, hitPoint.z);
                break;
            case GenericButtonListener.CONST_AXIS_Z:
                position = new Vector3(hitPoint.x, hitPoint.y, currentPosition.z);
                break;
            default:
                position = Vector3.zero;
                break;
        }

        if (position.y < MIN_Y)
            position = new Vector3(position.x, MIN_Y, position.z);
        if (position.y > MAX_Y)
            position = new Vector3(position.x, MAX_Y, position.z);

        return position;
    }

    Vector3 ScrollThingTopPosition(Vector3 scrollPosition)
    {
        return new Vector3(scrollPosition.x, MAX_Y, scrollPosition.z);
    }

    float GetScrollPercentage(Vector3 scrollPosition)
    {
        return (scrollPosition.y - MIN_Y) / (MAX_Y - MIN_Y);
    }

    float GetNumberOfLevelsHiddenBelow(Vector3 scrollPosition)
    {
        float rawCalculation = (GetScrollPercentage(scrollPosition) - (WINDOW_SIZE / (float)NUMBER_OF_LEVELS) / 2f) * NUMBER_OF_LEVELS;
        return Mathf.Min(Mathf.Max(0, rawCalculation), NUMBER_OF_LEVELS - WINDOW_SIZE);
    }

    float GetNumberOfLevelsHiddenAbove(Vector3 scrollPosition)
    {
        float rawCalculation = NUMBER_OF_LEVELS - GetNumberOfLevelsHiddenBelow(scrollPosition) - WINDOW_SIZE;
        return Mathf.Min(Mathf.Max(0, rawCalculation), NUMBER_OF_LEVELS - WINDOW_SIZE);
    }

    public static ScrollMechanics GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        levels = new ButtonObject[NUMBER_OF_LEVELS];
        for (int idx = 0; idx < NUMBER_OF_LEVELS; idx++)
        {
            levels[idx] = new ButtonObject(
                GameObject.Find("Level_" + idx),
                GameObject.Find("Level_" + idx + "Box"),
                GameObject.Find("Level_" + idx + "Text")
            );
        }

        boxMaterials = new Materials();
        boxMaterials.defaultMaterial = Resources.Load("MaterialsReferencedInScripts/TransparentTextBoxMat") as Material;
        boxMaterials.topDropdownMaterial = Resources.Load("MaterialsReferencedInScripts/TopDecomposeTexMat") as Material;
        boxMaterials.bottomDropdownMaterial = Resources.Load("MaterialsReferencedInScripts/BottomDecomposeTexMat") as Material;

        textMaterials = new Materials[NUMBER_OF_LEVELS];
        for (int idx = 0; idx < NUMBER_OF_LEVELS; idx++)
        {
            textMaterials[idx] = new Materials();
            textMaterials[idx].defaultMaterial = Resources.Load("MaterialsReferencedInScripts/TextsFade/Level_" + idx + "/DefaultMat") as Material;
            textMaterials[idx].topDropdownMaterial = Resources.Load("MaterialsReferencedInScripts/TextsFade/Level_" + idx + "/TopDecomposeMat") as Material;
            textMaterials[idx].bottomDropdownMaterial = Resources.Load("MaterialsReferencedInScripts/TextsFade/Level_" + idx + "/BottomDecomposeMat") as Material;
        }
    }

    void Start()
    {
        GenericButtonListener.InitializeButtons();

        box = GameObject.Find("ScrollBarBox");
        scrollCursor = GameObject.Find("ScrollBarCursor");

        button = GenericButtonListener.buttonsMap[GenericButtonListener.LOAD_LEVEL_0_BUTTON.id];
        scrollCursor.transform.position = PositionWithOffset(button.toggleAxis, button.greaterCoord);
        scrollCursor.transform.position = ScrollThingTopPosition(scrollCursor.transform.position);

        // There is a strange bug involved if we don't set the level gameObjects to inactive.
        // The bug affects the first two buttons (Level 0 and Level 1),
        // in the matter that both will keep the original black text material (the prefab material) until they go inactive and active back again.
        // The solution is to probably never assign materials to active gameObjects, but to activate them the next frame? Who knows...
        for (int idx = 0; idx < NUMBER_OF_LEVELS; idx++)
        {
            levels[idx]
                .parent
                .SetActive(false);
        }
    }

    void SetPositionOfButton(int globalIdx, int localIdx, int fadeModeOffset)
    {
        float y = HIGHEST_Y - Y_STEP * localIdx + unquantifyableOffset;// + fadeModeOffset * Y_STEP;
        levels[globalIdx].parent.transform.position = SetYPosition(levels[globalIdx].parent.transform.position, y);
    }

    void SetMaterial(int idx, int fadeMode)
    {
        float cutAmount;
        Material boxMaterial;
        Material textMaterial;
        PBRNoiseOffsetY cutDirectionBox;
        PBRNoiseOffsetY cutDirectionText;
        float PBRxOffset, PBRyOffsetBox, PBRyOffsetText;

        switch (fadeMode)
        {
            case OFFSET_NOT_FADED:
                levels[idx].box.GetComponent<MeshRenderer>().material = boxMaterials.defaultMaterial;
                levels[idx].text.GetComponent<MeshRenderer>().material = textMaterials[idx].defaultMaterial;
                return;
            case OFFSET_CUT_TOP:
                cutAmount = levelsHiddenAbove + 1 - firstCompletelyVisibleLevel;
                cutDirectionBox = TOP_CUT_BOX;
                cutDirectionText = TOP_CUT_TEXT;
                boxMaterial = boxMaterials.topDropdownMaterial;
                textMaterial = textMaterials[idx].topDropdownMaterial;
                break;
            case OFFSET_CUT_BOTTOM:
                cutAmount = Mathf.Ceil(levelsHiddenBelow) - levelsHiddenBelow;
                cutDirectionBox = BOTTOM_CUT_BOX;
                cutDirectionText = BOTTOM_CUT_TEXT;
                boxMaterial = boxMaterials.bottomDropdownMaterial;
                textMaterial = textMaterials[idx].bottomDropdownMaterial;
                break;
            default:
                return;
        }
        PBRxOffset = boxMaterial.GetVector("DropdownFade").x;
        PBRyOffsetBox = cutDirectionBox.start + cutAmount * (cutDirectionBox.end - cutDirectionBox.start);
        PBRyOffsetText = cutDirectionText.start + cutAmount * (cutDirectionText.end - cutDirectionText.start);
        boxMaterial.SetVector("DropdownFade", new Vector2(PBRxOffset, PBRyOffsetBox));
        textMaterial.SetVector("DropdownFade", new Vector2(PBRxOffset, PBRyOffsetText));

        levels[idx].box.GetComponent<MeshRenderer>().material = boxMaterial;
        levels[idx].text.GetComponent<MeshRenderer>().material = textMaterial;
    }


    bool IsCompletelyVisible(int idx)
    {
        return idx >= firstCompletelyVisibleLevel && idx <= lastCompletelyVisibleLevel;
    }
    bool IsCutOnTop(int idx)
    {
        return unquantifyableOffset != 0f && idx == firstCompletelyVisibleLevel - 1;
    }
    bool IsCutOnBottom(int idx)
    {
        return unquantifyableOffset != 0f && idx == lastCompletelyVisibleLevel + 1;
    }
    bool IsVisible(int idx)
    {
        return IsCompletelyVisible(idx) || IsCutOnTop(idx) || IsCutOnBottom(idx);
    }
    

    void Update()
    {
        ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f) && hit.transform.tag == "ScrollBox")
        {
            scrollCursor.transform.position = CalculateCurrentPosition(hit.point, scrollCursor.transform.position, button.toggleAxis);
        }

        levelsHiddenAbove = GetNumberOfLevelsHiddenAbove(scrollCursor.transform.position);
        levelsHiddenBelow = GetNumberOfLevelsHiddenBelow(scrollCursor.transform.position);

        int firstVisibleLevel = (int) Mathf.Floor(levelsHiddenAbove);
        firstCompletelyVisibleLevel = (int) Mathf.Ceil(levelsHiddenAbove);
        lastCompletelyVisibleLevel = NUMBER_OF_LEVELS - 1 - (int)Mathf.Ceil(levelsHiddenBelow);

        unquantifyableOffset = (Mathf.Ceil(levelsHiddenBelow) - levelsHiddenBelow) * Y_STEP;

        
        for (int idx = 0; idx < NUMBER_OF_LEVELS; idx++)
        {
            int fadeModeOffset = IsCompletelyVisible(idx) ? 0 :
                                    IsCutOnBottom(idx) ? -1 :
                                    1;
            if (IsVisible(idx))
            {
                levels[idx].parent.SetActive(true);
                SetPositionOfButton(idx, idx - firstVisibleLevel, fadeModeOffset);
                SetMaterial(idx, fadeModeOffset);
            }
            else
            {
                levels[idx]
                    .parent
                    .SetActive(false);
            }
        }
    }
}
