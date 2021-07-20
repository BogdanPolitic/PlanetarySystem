using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    const int UPWARDS = 0;
    const int DOWNWARDS = 1;
    const int LEFTWARDS = 2;
    const int RIGHTWARDS = 3;

    int idx;
    Vector3[] circle, equationCircle;
    Vector3 center3d;
    bool keyUpped;
    int framesAfterUpped;
    int exp;
    int slowDownSide;
    bool movedVertically;
    int deltaHeight; // +- 1 each frame
    int logarithmicEnding;
    int lastUpFrame;
    public float lowestHeight;
    public float slopeSteepness;
    public float speed;
    public bool showLowestHeight;

    Vector3 MiddleOfSegment(Vector3 p1, Vector3 p2)
    {
        return new Vector3((p1.x + p2.x) / 2, p1.y, (p1.z + p2.z) / 2);
    }

    /// <summary>
    /// pointeaza spre centru, adica returneaza un vector cu orientarea spre centrul cercului
    /// </summary>
    Vector3 Orientate(int index, Vector3[] circle, Vector3 center3d)
    {
        return center3d - circle[index];
    }

    Vector2 SolvePosition(float radius, float angle, Vector3 center3d, int idx)
    {
        if (angle == 0.5f)
        {
            return new Vector2(0, radius);
        }
        if (angle == 1.5f)
            return new Vector2(0, -radius);

        float tan = Mathf.Tan(angle * Mathf.PI);
        float x = radius / Mathf.Sqrt(1 + Mathf.Pow(tan, 2));
        x = angle < 0.5f || angle > 1.5f ? x : -x;
        float z = tan * x;

        x += center3d.x;
        z += center3d.y;

        return new Vector2(x, z);
    }
    Vector3[] HorizontalCircle(float radius, float height, Vector3 center2d, int vertexNo)
    {
        Vector3[] circle = new Vector3[vertexNo + 1];
        float angleBetween = (float)2 / vertexNo; // 2 vine de la (360 = 2 * pi)

        for (int idx = 0; idx < vertexNo; idx++)
        {
            Vector2 xz = SolvePosition(radius, idx * angleBetween, center2d, idx);
            circle[idx] = new Vector3(xz.x, height, xz.y);
        }
        circle[vertexNo] = circle[0];

        return circle;
    }

    void PrepareToSlowDown()
    {
        keyUpped = true;
        framesAfterUpped = 1;
        exp = 1;
    }

    void SlowDownIfKeyUpped(int side) // for leftwards / rightwards input
    {
        if (keyUpped)
        {
            if (framesAfterUpped % Mathf.Pow(2, exp) == 0)
            {
                transform.position = circle[idx];
                transform.rotation = Quaternion.LookRotation(Orientate(idx, circle, center3d), Vector3.up);
                if (side == RIGHTWARDS)
                    idx = (idx + 1) % (circle.Length - 1);
                if (side == LEFTWARDS)
                    idx = (idx + circle.Length - 2) % (circle.Length - 1);

                exp++;
            }
            if (exp == 4)
            {
                keyUpped = false;
            }
            framesAfterUpped++;
        }
    }

    void Move(int side)
    {
        if (movedVertically)
        {
            for (int i = 0; i < circle.Length; i++)
            {
                circle[i] = ActualizeHeightForIndex(i, side);
            }
            movedVertically = false;
            deltaHeight = 0;
            ActualizeTransform(idx);
            return;
        }

        ActualizeTransform(idx);

        if (side == RIGHTWARDS)
            idx = (idx + 1) % (circle.Length - 1);

        if (side == LEFTWARDS)
            idx = (idx + circle.Length - 2) % (circle.Length - 1);

        slowDownSide = side;
    }

    void ActualizeTransform(int index)
    {
        transform.position = circle[index];
        transform.rotation = Quaternion.LookRotation(Orientate(index, circle, center3d), Vector3.up);
    }

    Vector3 ActualizeHeightForIndex(int index, int side)
    {
        Transform oldTransform = transform;
        ActualizeTransform(index); // setez gameObject.transform-ul (la fiecare apel al acestei functii, adica la fiecare index din circle) pentru a putea folosi eulerAngles.y, dar apoi il resetez inapoi, si astfel nu se afecteaza nimic.
        Vector2 stopPoint = new Vector2(center3d.x + Mathf.Cos(3 * Mathf.PI / 2 - gameObject.transform.rotation.eulerAngles.y / 180 * Mathf.PI), center3d.z + Mathf.Sin(3 * Mathf.PI / 2 - gameObject.transform.rotation.eulerAngles.y / 180 * Mathf.PI));
        transform.position = oldTransform.position; //
        transform.rotation = oldTransform.rotation; // resetarea inapoi a transform-ului camerei.
        float norm = (stopPoint - new Vector2(transform.position.x, transform.position.z)).magnitude;
        float slowDownFactor = norm < 1 && side == UPWARDS ? norm : 1;
        //slowDownFactor = logarithmicEnding == 0 && (circle[index] - center3d).magnitude < (equationCircle[index] - center3d).magnitude ? slowDownFactor : slowDownFactor * logarithmicEnding / 8; // stiu ca normal trebuia / 4, but to slow the y-axis ascending moe
        slowDownFactor = slowDownFactor < 0.001f ? 0 : slowDownFactor;

        float x = circle[index].x + deltaHeight * speed * (equationCircle[index].x - stopPoint.x) * slowDownFactor;
        float z = circle[index].z + deltaHeight * speed * (equationCircle[index].z - stopPoint.y) * slowDownFactor;
        float y = lowestHeight + (Mathf.Pow(x - equationCircle[index].x, 2) + Mathf.Pow(z - equationCircle[index].z, 2)) * slopeSteepness;
        //if (index == idx)
        //    print("sl = " + slowDownFactor);

        return new Vector3(x, y, z);
    }

    void DrawLine(Vector3[] trj, Color run)
    {
        if (GetComponent<LineRenderer>() == null)
            gameObject.AddComponent<LineRenderer>();
        GetComponent<LineRenderer>().widthMultiplier = 0.02f;
        GetComponent<LineRenderer>().material.SetColor("_Color", run);
        GetComponent<LineRenderer>().positionCount = trj.Length;
        for (int i = 0; i < trj.Length; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, trj[i]);
        }
    }

    private void Awake()
    {
        center3d = new Vector3(0, 0, 0);
        idx = 0;
        keyUpped = false;
        framesAfterUpped = 0;
        lastUpFrame = 0;
        exp = 0;
        movedVertically = false;
        deltaHeight = 0;
        logarithmicEnding = 0;

        lowestHeight = 2;
        speed = 0.02f;
        slopeSteepness = 0.4f;

        showLowestHeight = false;
    }

    void Start()
    {
        circle = HorizontalCircle(8, 2, center3d, 199);
        equationCircle = HorizontalCircle(8, lowestHeight, center3d, 199);
        ActualizeTransform(0); // initial

        if (showLowestHeight)
        {
            LineRenderer lineR = gameObject.AddComponent<LineRenderer>();
            lineR.widthMultiplier = 0.02f;
            lineR.material.SetColor("_Color", Color.red);
            lineR.positionCount = 200;
            for (idx = 0; idx < lineR.positionCount; idx++)
            {
                lineR.SetPosition(idx, circle[idx]);
            }
        }

        idx = 0;
    }

    void Update()
    {
        if (Input.GetKey("left"))
            Move(LEFTWARDS);
        else if (Input.GetKeyUp("left"))
            PrepareToSlowDown();

        if (Input.GetKey("right"))
            Move(RIGHTWARDS);
        else if (Input.GetKeyUp("right"))
            PrepareToSlowDown();

        SlowDownIfKeyUpped(slowDownSide);
        
        if (Input.GetKey("down") || Input.GetKey("up"))
        {
            deltaHeight = Input.GetKey("down") ? 1 : -1;
            movedVertically = true;
            
            Move(Input.GetKey("down") ? DOWNWARDS : UPWARDS);
        }
        if (Input.GetKeyUp("down") || Input.GetKeyUp("up"))
        {
            logarithmicEnding = Input.GetKeyUp("down") ? 3 : -3;
            lastUpFrame = Time.frameCount;
        }

        if (logarithmicEnding < 0 && Time.frameCount - lastUpFrame == Mathf.Pow(2, 5 - Mathf.Abs(logarithmicEnding)))
        {
            deltaHeight = -1;
            movedVertically = true;
            
            Move(UPWARDS);
            logarithmicEnding++;
        }
        else if (logarithmicEnding > 0 && Time.frameCount - lastUpFrame == Mathf.Pow(2, 5 - Mathf.Abs(logarithmicEnding)))
        {
            deltaHeight = 1;
            movedVertically = true;
            
            Move(DOWNWARDS);
            logarithmicEnding--;
        }

        if (Input.GetKeyDown("e"))
        {
            DrawLine(circle, Color.red);
        }
    }
}