using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.EventSystems;

public class PathReader1 : MonoBehaviour
{

    class Oposed
    {
        public int from; // idx
        public int to; // idx
        public float dst;
    }

    class Segment
    {
        public Vector3 p1;
        public Vector3 p2;
        public float m;
        public Segment(Vector3 _p1, Vector3 _p2)
        {
            p1 = _p1;
            p2 = _p2;
            m = (p2.z - p1.z) / (p2.x - p1.x);
        }
    }

    class Intersect
    {
        public bool inInterval;
        public float x;
        public Intersect(bool _inInterval, float _x)
        {
            inInterval = _inInterval;
            x = _x;
        }
    }

    class Vertex
    {
        public int index;
        public Vector3 oldPosition;
        public Vertex(int _index, Vector3 _oldPosition)
        {
            index = _index;
            oldPosition = _oldPosition;
        }
    }

    class AllowedShape
    {
        public bool allowed;
        public Vector3[] shape;
        public AllowedShape(bool _allowed, Vector3[] _shape)
        {
            allowed = _allowed;
            shape = _shape;
        }
    }

    public class Planet
    {
        public Inventory.PlanetStack item;
        public GameObject planet;
        public GameObject collisionChecker;
        public Vector3[] trajectory;
        public int rotationIndex;
        public bool deleteMark;
        public HalfspherePair collisionParticles;
        public int orderIndex;
        public RenderSwitch switchRendering;
        public int startFrame;
        public Vector3 lastFrameVelocity;
        public Planet(Inventory.PlanetStack _item, GameObject _planet, Vector3[] _trajectory, int _rotationIndex, int _orderIndex, int _startFrame, int _ordIdx)
        {
            item = _item;
            planet = _planet;
            planet.SetActive(true);
            trajectory = _trajectory;
            rotationIndex = _rotationIndex;
            deleteMark = false;
            collisionParticles = null;
            startFrame = _startFrame;
            lastFrameVelocity = Vector3.zero;
            orderIndex = _orderIndex;

            planet.transform.localScale *= (item.size / 10.0f);

            collisionChecker = Instantiate(planet);
            collisionChecker.GetComponent<MeshRenderer>().enabled = false;
            collisionChecker.GetComponent<SphereCollider>().isTrigger = true;
            Destroy(planet.GetComponent<Checker>());
            Destroy(planet.GetComponent<SphereCollider>());

            switchRendering = new RenderSwitch(false, 0, false);
            
            planet.GetComponent<Rigidbody>().mass = 1 + (float) _ordIdx / 100;
            collisionChecker.GetComponent<Rigidbody>().mass = 1 + (_ordIdx + 0.5f) / 100;

            //textureIdx = randomGen.Next(0, TexturePath.units.Length - 1);
            //Texture planetTex = Resources.Load("PlanetSurfaces/Unit/" + TexturePath.units[textureIdx]) as Texture;
            //planet.GetComponent<MeshRenderer>().material.mainTexture = planetTex;
            Texture planetTex = Resources.Load("PlanetSurfaces/Unit/" + item.name) as Texture;
            planet.GetComponent<MeshRenderer>().material.mainTexture = planetTex;
        }
    }

    static class TexturePath
    {
        public static string[] units = new string[]
        {
            "bluesky",
            "earth",
            "jupiter",
            "mars",
            "mercury",
            "neptune",
            "saturn",
            "uranus",
            "venus",
            "pluto"
        };

        public static string[] halfs = new string[]
        {
            "bottom_half_bluesky",
            "bottom_half_earth",
            "bottom_half_jupiter",
            "bottom_half_mars",
            "bottom_half_mercury",
            "bottom_half_neptune",
            "bottom_half_saturn",
            "bottom_half_uranus",
            "bottom_half_venus_orange",
            "bottom_half_venus_skinny",
            "top_half_bluesky",
            "top_half_earth",
            "top_half_jupiter",
            "top_half_mars",
            "top_half_mercury",
            "top_half_neptune",
            "top_half_saturn",
            "top_half_uranus",
            "top_half_venus",
            "top_half_pluto"
        };
    }

    class MemorizedPositions
    {
        public List<int> orbitingPlanets;
        public List<int> freeMotionPlanets;
        public MemorizedPositions()
        {
            orbitingPlanets = new List<int>();
            freeMotionPlanets = new List<int>();
        }
    }

    public class Halfsphere
    {
        public GameObject hs;
        public GameObject collisionChecker;
        public int createdAtFrame;
        public Vector3 lastFrameVelocity;
        public float fade;
        public bool deleteMark;

        private const int DESTRUCTION_START = 5;    // seconds
        private const int DESTRUCTION_END = 10;     // seconds
        public float fadeUnity = (DESTRUCTION_END - DESTRUCTION_START) * Time.deltaTime;

        public Halfsphere(GameObject _hs, Inventory.PlanetStack fromPlanetItem, int _ordIdx)
        {
            hs = _hs;

            hs.transform.localScale *= (fromPlanetItem.size / 10.0f);

            collisionChecker = Instantiate(hs);
            collisionChecker.GetComponent<MeshRenderer>().enabled = false;
            collisionChecker.GetComponent<MeshCollider>().isTrigger = true;
            Destroy(hs.GetComponent<Checker>());

            hs.GetComponent<Rigidbody>().mass = 1 + (float) _ordIdx / 100;
            collisionChecker.GetComponent<Rigidbody>().mass = 1 + (_ordIdx + 0.5f) / 100;

            createdAtFrame = realFrameCount;
            lastFrameVelocity = Vector3.zero;
            fade = 1f;
            deleteMark = false;
        }

        public float age()
        {
            return (realFrameCount - createdAtFrame) * Time.deltaTime;
        }

        public int status()
        {
            if (age() < DESTRUCTION_START)
                return INTEGRAL;
            else if (age() < DESTRUCTION_END)
                return IN_DESTRUCTION;
            else
                return DESTRUCTED;
        }
    }

    public class HalfspherePair
    {
        public GameObject hs1;
        public GameObject hs2;
        public HalfspherePair(Inventory.PlanetStack fromPlanetItem)
        {
            string sourcePlanetName = fromPlanetItem.name;

            hs1 = Instantiate(GameObject.Find("Halfsphere"));
            hs1.transform.Rotate(90, 0, 0);
            Texture hs1Tex = Resources.Load("PlanetSurfaces/Half/bottom_half_" + sourcePlanetName) as Texture;
            hs1.GetComponent<MeshRenderer>().material.mainTexture = hs1Tex;

            hs2 = Instantiate(GameObject.Find("Halfsphere"));
            hs2.transform.Rotate(90, 0, 0);
            Texture hs2Tex = Resources.Load("PlanetSurfaces/Half/top_half_" + sourcePlanetName) as Texture;
            hs2.GetComponent<MeshRenderer>().material.mainTexture = hs2Tex;
        }
    }

    public class RenderSwitch
    {
        public bool allowed;
        public int switchesLeft;
        public int lastSwitchFrame;
        public bool dissapearThen;
        public RenderSwitch(bool _allowed, int _switchesLeft, bool _dissapearThen)
        {
            allowed = _allowed;
            switchesLeft = _switchesLeft;
            lastSwitchFrame = realFrameCount;
            dissapearThen = _dissapearThen;
        }
    }

    class FlickeringLine
    {
        public int switchesLeft;
        public int framesBetweenSwitches;
        public GameObject gO;
        public FlickeringLine(int _switchesLeft, int _framesBetweenSwitches, GameObject _gO)
        {
            switchesLeft = _switchesLeft;
            framesBetweenSwitches = _framesBetweenSwitches;
            gO = _gO;
        }
    }

    /*void setBaseOpacity(float opacity)
    {
        Renderer r = GetComponent<Renderer>();
        Color color = r.material.color; color.a = opacity;
        r.material.color = color;
    }*/

    void DrawLine(Vector3 p1, Vector3 p2, Color run)
    {
        if (GetComponent<LineRenderer>() == null)
            gameObject.AddComponent<LineRenderer>();
        GetComponent<LineRenderer>().widthMultiplier = 0.02f;
        GetComponent<LineRenderer>().material.SetColor("_Color", run);
        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, p1);
        GetComponent<LineRenderer>().SetPosition(1, p2);
    }

    GameObject ShowTrajectory(Vector3[] trajectory, Color color)
    {
        //GameObject gO = Instantiate(gameObject); // bloc copiat pentru a ii pune in sarcina lui (copiei) linerenderer-ul
        GameObject gO = new GameObject();
        LineRenderer lineRenderer;

        gO.name = "traj";
        Destroy(gO.GetComponent<PathReader1>());
        Destroy(gO.GetComponent<MeshRenderer>());
        Destroy(gO.GetComponent<BoxCollider>());
        
        if (gO.GetComponent<LineRenderer>() == null)
        {
            lineRenderer = gO.AddComponent<LineRenderer>();
        }
        else
        {
            lineRenderer = gO.GetComponent<LineRenderer>();
        }

        lineRenderer.material = new Material(Shader.Find("Nature/SpeedTree")); // LINIA ASTA CAUZEAZA PROBLEME PE ANDROID (SHADER-UL) !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        lineRenderer.material.color = color;
        lineRenderer.widthMultiplier = 0.1f;        // ASTFEL AM DEZACTIVAT LINERENDERER-UL !!! INAINTE ERA VIZIBIL, PENTRU CA AVEA WIDTH MULTIPLIER = 0.2F (ACUM L-AM PUS PE 0.0F CA SA FIE INACTIV)
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount += trajectory.Length;
        for (int idx = 0; idx < trajectory.Length; idx++)
        {
            lineRenderer.SetPosition(lineRenderer.positionCount - trajectory.Length + idx, trajectory[idx]);
        }

        //
        var child_transform = gameObject.transform;
        gO.transform.SetParent(gameObject.transform);
        gO.transform.position = child_transform.position;
        gO.transform.rotation = child_transform.rotation;
        //

        return gO;
    }

    void SetTrajectoryHeight(List<Vector3> trajectory, float height)
    {
        for (int idx = 0; idx < trajectory.Count; idx++)
        {
            trajectory[idx] = new Vector3(trajectory[idx].x, height, trajectory[idx].z);
        }
    }

    /// <summary>
    /// Rotates the array forward (to the right)!
    /// </summary>
    T[] RotateArray<T>(T[] array, int units)
    {
        T[] rotatedArray = new T[array.Length];
        for (int idx = units; idx < array.Length; idx++)
        {
            rotatedArray[idx - units] = array[idx];
        }
        for (int idx = 0; idx < units; idx++)
        {
            rotatedArray[array.Length - units + idx] = array[idx];
        }
        return rotatedArray;
    }

    bool InOpenInterval(float x, float a, float b)
    {
        return ((a > b && x > b && x < a) || (a < b && x > a && x < b));
    }

    bool InInterval(float x, float a, float b)
    {
        return ((a >= b && x >= b && x <= a) || (a <= b && x >= a && x <= b));
    }

    /// <summary>
    /// Infinite line intersects finite segment (returns true), or it doesn't (returns false). Based on solving the 2 equations system of 2D intersection, and finding out if x is in the correct range.
    /// </summary>
    Intersect LineIntersectsSegment(Segment line, Segment seg)
    {
        if (seg.p1 == seg.p2)
        {
            return LineCrossesPoint(line, seg.p1);
        }

        float x = seg.p1.x == seg.p2.x ? seg.p1.x : (seg.p2.z - line.p2.z - seg.m * seg.p2.x + line.m * line.p2.x) / (line.m - seg.m);
        return new Intersect((seg.p1.x == seg.p2.x && InInterval(line.p2.z + line.m * (seg.p2.x - line.p2.x), seg.p1.z, seg.p2.z)) || (seg.p1.x != seg.p2.x && InInterval(x, seg.p1.x, seg.p2.x)), x);
    }
    Intersect LineIntersectsOpenSegment(Segment line, Segment seg)
    {
        if (seg.p1 == seg.p2)
        {
            return LineCrossesPoint(line, seg.p1);
        }
        float x = (seg.p2.z - line.p2.z - seg.m * seg.p2.x + line.m * line.p2.x) / (line.m - seg.m);
        return new Intersect((seg.p1.x == seg.p2.x && InOpenInterval(line.p2.z + line.m * (seg.p2.x - line.p2.x), seg.p1.z, seg.p2.z)) || (seg.p1.x != seg.p2.x && InOpenInterval(x, seg.p1.x, seg.p2.x)), x);
    }

    Intersect LineCrossesPoint(Segment line, Vector3 point)
    {
        float m = (line.p2.z - line.p1.z) / (line.p2.x - line.p1.x);
        return new Intersect(point.z == line.p2.z + m * (point.x - line.p2.x), point.x);
    }

    bool LineIntersectsAnySegment(Segment line, Vector3[] polygon)
    {
        for (int idx = 0; idx < polygon.Length - 1; idx++)
        {
            if (LineIntersectsSegment(line, new Segment(polygon[idx], polygon[idx + 1])).inInterval)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if point (second argument) is placed inside a polygon (determined by a vector of points). Applies to axis X and Z; the Y axis does not matter. Algorithm: creating a (random) line which passes the point, and checking if it intersects the borders of the polygon and if it does, checking if the X value of the point is in the correct range.
    /// </summary>
    bool IsInConvexPolygon(Vector3[] polygon, Vector3 point)
    {
        for (int idx = 0; idx < polygon.Length; idx++)
        {
            if (point == polygon[idx])
            {
                return true;
            }
        }

        List<Vertex> CrossedVertices = new List<Vertex>(); // lista de varfuri vechi, pentru backup-ul care se restoreaza la finalul acestei functii
        Segment line = new Segment(new Vector3(2 * point.x - 100, point.y, 2 * point.z - 100), new Vector3(100, 0, 100));
        for (int idx = 0; idx < polygon.Length; idx++)
        {
            if (LineCrossesPoint(line, polygon[idx]).inInterval) // impropriu-spus "inInterval", pentru intersectia punctelor. era propriu-spus pentru inters dreptelor
            {
                CrossedVertices.Add(new Vertex(idx, polygon[idx]));
                polygon[idx] = new Vector3(polygon[idx].x, polygon[idx].y, polygon[idx].z + 0.1f);
            }
        }

        List<Intersect> intersectionPointsList = new List<Intersect>();
        Intersect[] intersectionPointsArray;
        for (int idx = 0; idx < polygon.Length; idx++)
        {
            Intersect intersect = LineIntersectsSegment(line, new Segment(polygon[idx], polygon[(idx + 1) % polygon.Length]));
            if (intersect.inInterval)
            {
                intersectionPointsList.Add(intersect);
            }
        }
        intersectionPointsArray = intersectionPointsList.ToArray();

        bool isIn = false;
        if (intersectionPointsArray.Length % 2 == 0)
        {
            for (int idx2 = 0; idx2 < intersectionPointsArray.Length / 2; idx2++)
            {
                isIn |= InInterval(point.x, intersectionPointsArray[2 * idx2].x, intersectionPointsArray[2 * idx2 + 1].x);
            }
        }
        foreach (Vertex vertex in CrossedVertices)
        {
            polygon[vertex.index] = vertex.oldPosition;
        }

        return isIn;
    }

    const int leftSide = 0;
    const int rightSide = 1;

    // intoarce "proiectia" punctului pe dreapta to-from, "proiectie" realizata doar in cadrul axei Oz (coordonata x ramane aceeasi)
    // de fapt aceasta functie aduce toate punctele din -> dreapta segmentului to-from direct pe segmentul to-from (pentru arg rightSide)
    //                                                  -> stanga segmentului to-from direct pe segmentul to-from (pentru arg leftSide)
    Vector3 LineApproximation(Vector3 from, Vector3 to, Vector3 point, int approximateIf)
    {
        float m = (from.z - to.z) / (from.x - to.x);
        float zOnLine = from.z + m * (point.x - from.x);

        if (approximateIf == leftSide && ((point.z < zOnLine && m < 0) || (point.z > zOnLine && m > 0))) //                   *\         sau        */
        {
            return new Vector3(point.x, point.y, zOnLine);
        }
        if (approximateIf == rightSide && ((point.z > zOnLine && m < 0) || (point.z < zOnLine && m > 0))) //                \*        sau        /*                
        {
            return new Vector3(point.x, point.y, zOnLine); // same thing as above
        }
        return point;
    }

    Vector3 LineApproximation(Vector3 from, Vector3 to, Vector3 point)
    {
        float m = (from.z - to.z) / (from.x - to.x);
        float zOnLine = from.z + m * (point.x - from.x);

        return new Vector3(point.x, point.y, zOnLine);
    }

    bool PointTowardsInterior(Vector3[] polygon, int pointIndex)
    {
        Vector3 oldPoint = polygon[pointIndex];
        polygon[pointIndex] = LineApproximation(polygon[(pointIndex - 1 + polygon.Length) % polygon.Length], polygon[(pointIndex + 1) % polygon.Length], polygon[pointIndex]);

        bool ret = IsInConvexPolygon(polygon, oldPoint);
        polygon[pointIndex] = oldPoint;
        return ret;
    }

    void FlattenInnerCurve(List<Vector3> polygon, int pointIndex)
    {
        if (PointTowardsInterior(polygon.ToArray(), pointIndex))
        {
            polygon.RemoveAt(pointIndex);
            FlattenInnerCurve(polygon, (pointIndex - 1 + polygon.Count) % polygon.Count);
            FlattenInnerCurve(polygon, pointIndex % polygon.Count);
        }
    }

    Vector3[] FlattenInnerCurves(List<Vector3> polygon)
    {
        for (int idx = 0; idx < polygon.Count; idx++)
        {
            FlattenInnerCurve(polygon, idx);
        }
        return polygon.ToArray();
    }

    Vector3[] PointsAtUniformDistance(List<Vector3> polygon) // WORKS 100% !! - uses optimalDst
    {
        int idx = 0;
        while (idx < polygon.Count - 1)
        {
            Vector3 from = polygon[idx];
            Vector3 to = polygon[(idx + 1) % polygon.Count];
            float dst = (to - from).magnitude;

            if (dst > optimalDst)
            {
                for (int i = 1; i < Mathf.Floor(dst / optimalDst); i++)
                {
                    polygon.Insert(idx + i, new Vector3(from.x + (to.x - from.x) * i * optimalDst / dst, /*polygon[idx + i - 1].y*/from.y + (to.y - from.y) * i * optimalDst / dst, from.z + (to.z - from.z) * i * optimalDst / dst));
                }
                idx += (int)Mathf.Floor(dst / optimalDst);
            }
            else idx++;
        }

        return polygon.ToArray();
    }

    Vector3[] EquidistantVertex(Vector3[] markersV) // DOES NOT REALLY WORK !! - uses interp
    {
        List<Vector3> positions = new List<Vector3>();
        for (int idx = 0; idx < markersV.Length - 1; idx++)
        {
            for (int iter = 0; iter < interp; iter++)
            {
                float posX = markersV[idx].x + (markersV[idx + 1].x - markersV[idx].x) / interp * iter;
                float posY = markersV[idx].y + (markersV[idx + 1].y - markersV[idx].y) / interp * iter + 1; // ca sa fie mai sus decat suprafata
                float posZ = markersV[idx].z + (markersV[idx + 1].z - markersV[idx].z) / interp * iter;
                positions.Add(new Vector3(posX, posY, posZ));
            }
        }
        positions.Add(new Vector3(markersV[markersV.Length - 1].x, markersV[markersV.Length - 1].y + 1, markersV[markersV.Length - 1].z)); // last point (establishing continuity & circularity)
        return positions.ToArray();
    }

    Vector3[] CloseShape(Vector3[] polygon)
    {
        List<Vector3> list = VectorToList(polygon);
        list.Add(polygon[0]);
        return list.ToArray();
    }

    float DistancePointLine(Segment line, Vector3 point)
    {
        if (line.p1.x == line.p2.x)
        {
            return Mathf.Abs(line.p2.x - point.x);
        }
        float a, b, c;
        a = (line.p2.z - line.p1.z) / (line.p2.x - line.p1.x);
        b = -1;
        c = line.p2.z - a * line.p2.x;

        return Mathf.Abs(a * point.x + b * point.z + c) / Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
    }

    Vector3 MiddleOfSegment(Segment segment)
    {
        return new Vector3((segment.p1.x + segment.p2.x) / 2, (segment.p1.y + segment.p2.y) / 2, (segment.p1.z + segment.p2.z) / 2);
    }

    void SmoothenAtPoint(Vector3[] polygon, float optimal, int index, int dpt, int maxdpt)
    {
        if (dpt == maxdpt)
        {
            return;
        }
        float optimalDistance = optimal * optimalDst;
        int previousIndex = (index - 1 + polygon.Length) % polygon.Length;
        int nextIndex = (index + 1) % polygon.Length;

        if (DistancePointLine(new Segment(polygon[previousIndex], polygon[nextIndex]), polygon[index]) > optimalDistance)
        {
            polygon[index] = MiddleOfSegment(new Segment(polygon[previousIndex], polygon[nextIndex]));
            if (index > 0)
                SmoothenAtPoint(polygon, optimal, previousIndex, dpt + 1, maxdpt);
            if (index < polygon.Length - 1)
                SmoothenAtPoint(polygon, optimal, nextIndex, dpt + 1, maxdpt);
        }
    }

    Vector3[] SmoothenBounds(Vector3[] polygon, float optimal)
    {
        for (int idx = 1; idx < polygon.Length; idx++)
        {
            SmoothenAtPoint(polygon, optimal, idx, 0, 20);
        }
        polygon = polygon.Take(polygon.Length - 1).ToArray();
        polygon = RotateArray(polygon, polygon.Length / 2);
        polygon[polygon.Length - 1] = polygon[0];
        for (int idx = 1; idx < polygon.Length; idx++)
        {
            SmoothenAtPoint(polygon, optimal, idx, 0, 20);
        }
        polygon = polygon.Take(polygon.Length - 1).ToArray();
        polygon = RotateArray(polygon, polygon.Length - polygon.Length / 2);
        polygon[polygon.Length - 1] = polygon[0];
        return polygon;
    }

    AllowedShape AllowToCloseShape(Vector3[] polygon)
    {
        float deviationsSum = 0;
        int idx;
        if (polygon[polygon.Length - 1] == polygon[0])
        {
            polygon = polygon.Take(polygon.Length - 1).ToArray();
        }
        for (idx = 1; idx < polygon.Length - 1; idx++)
        {
            if (deviationsSum + 180 - AngleBetweenPoints(polygon[idx - 1], polygon[idx], polygon[idx + 1]) > 355)
            {
                break;
            }
            deviationsSum += 180 - AngleBetweenPoints(polygon[idx - 1], polygon[idx], polygon[idx + 1]);
        }
        if (deviationsSum < minimumCompletionAngle)
            return new AllowedShape(false, polygon);
        return new AllowedShape(true, polygon.Take(idx - 1).ToArray());
    }

    /// <summary>
    /// Does NOT update the planet.rotationIndex!
    /// </summary>
    int NextIndex(Planet planet)
    {
        int index = (planet.rotationIndex + 1) % planet.trajectory.Length;
        return index;
    }

    int NextIndex(Planet planet, int X)
    {
        int index = (planet.rotationIndex + X) % planet.trajectory.Length;
        return index;
    }

    /// <summary>
    /// Returneaza lista de lungime 2, daca colizioneaza cu o planeta, sau lista de lungime 1, daca colizioneaza cu o semisfera!
    /// </summary>
    List<int> CollisionListXFramesLater(int X, List<Planet> planets, List<Halfsphere> freeMotionPlanets, int currentPlanetIndex, int rotationIndex)
    {
        List<int> toDisintegrateIndexList = new List<int>();
        Planet newPlanet = null; // in caz ca planeta a fost pusa pe scena cu cel mult 1 frame in urma

        if (planets[currentPlanetIndex].collisionChecker.GetComponent<Checker>().collidedWithPlanet) // planeta curenta colizioneaza cu o planeta
        {
            GameObject colliderObj = planets[currentPlanetIndex].collisionChecker.GetComponent<Checker>().colliderObj; // in fact, the checker of it (of the colliderObj)
            Planet collidedPlanet = planets.Where(planet => planet.collisionChecker.transform.position == colliderObj.transform.position).ToArray()[0]; // in fact, the checker of it
            int collidedPlanetIndex = planets.IndexOf(collidedPlanet);
            
            // daca cel putin una din cele doua planete a fost nou creata (cu maxim 1 frame inainte):
            if (realFrameCount - planets[currentPlanetIndex].startFrame <= 1 || realFrameCount - collidedPlanet.startFrame <= 1)
            {
                newPlanet = realFrameCount - planets[currentPlanetIndex].startFrame <= 1 ? planets[currentPlanetIndex] : collidedPlanet;
            }

            // aceeasi conditie ca la if-ul trecut (daca cel putin una din cele doua planete a fost nou creata):
            if (newPlanet != null)
            {
                SwitchRendering(newPlanet);
                planets[currentPlanetIndex].collisionChecker.GetComponent<Checker>().collidedWithPlanet = false; // asta trebuie sa 'sufere' ambele planete colizionate, daca cel putin una e nou aparuta in scena
                return new List<int>();
            }

            toDisintegrateIndexList.Add(currentPlanetIndex);
            toDisintegrateIndexList.Add(collidedPlanetIndex);
        }

        if (planets[currentPlanetIndex].collisionChecker.GetComponent<Checker>().collidedWithHalfsphere) // planeta curenta colizioneaza cu o semisfera
        {
            newPlanet = realFrameCount - planets[currentPlanetIndex].startFrame <= 1 ? planets[currentPlanetIndex] : null;
            // daca planeta ce urmeaza sa intre in coliziune este nou creata (a fost creata cu cel mult 1 frame in urma):
            if (newPlanet != null)
            {
                SwitchRendering(newPlanet);
                planets[currentPlanetIndex].collisionChecker.GetComponent<Checker>().collidedWithHalfsphere = false; // sufera doar planeta; semisfera nu sufera nimic.
                return new List<int>();
            }

            toDisintegrateIndexList.Add(currentPlanetIndex);
        }
        
        return toDisintegrateIndexList;
    }

    Vector3 MotionToVelocity(Vector3 lastFrame, Vector3 currentFrame)
    {
        return (currentFrame - lastFrame) / Time.deltaTime;
    }

    Vector3 MotionToVelocity(Vector3[] trajectory, int currentIdx)
    {
        int lastIdx = (currentIdx + trajectory.Length - 1) % trajectory.Length; // (14.03.2020) lastIdx este de fapt previous index, nu?

        return MotionToVelocity(trajectory[lastIdx], trajectory[currentIdx]);
    }

    float AngleAdapted(GameObject g)
    {
        Vector3 v = g.GetComponent<Rigidbody>().velocity;
        float unsignedAngle = Vector3.Angle(v, Vector3.right);
        if (v.z >= 0)
        {
            return 180 - unsignedAngle;
        }
        else
        {
            return 180 + unsignedAngle;
        }
    }

    float AngleAdapted(GameObject g, Vector3 velocity)
    {
        float unsignedAngle = Vector3.Angle(velocity, Vector3.right);
        if (velocity.z >= 0)
        {
            return 180 - unsignedAngle;
        }
        else
        {
            return 180 + unsignedAngle;
        }
    }

    void RotationAlongVelocitySide1(GameObject g, Vector3 velocity)
    {
        g.transform.rotation = Quaternion.Euler(g.transform.rotation.x, AngleAdapted(g, velocity), g.transform.rotation.z);
    }

    void RotationAlongVelocitySide2(GameObject g, Vector3 velocity)
    {
        g.transform.rotation = Quaternion.Euler(g.transform.rotation.x, AngleAdapted(g, velocity) + 180, g.transform.rotation.z);
    }

    Vector3 HalfspheresCoords(Vector3 instPos, Vector3 velocity, bool upwards, bool firstObj)
    {
        float velocityDirection = Mathf.Atan2(velocity.z, velocity.x);
        float deltaX = hsInBetweenDistance / 2 * Mathf.Cos(velocityDirection);
        float deltaZ = hsInBetweenDistance / 2 * Mathf.Sin(velocityDirection);
        float xc = (upwards && firstObj) || (!upwards && !firstObj) ? instPos.x - deltaX : instPos.x + deltaX;
        float zc = (upwards && firstObj) || (!upwards && !firstObj) ? instPos.z + deltaZ : instPos.z - deltaZ;

        return new Vector3(xc, instPos.y, zc);
    }

    HalfspherePair Disintegrate(Planet planet, Vector3 velocity)
    {
        GameObject gameObj = planet.planet;
        HalfspherePair hsPair = new HalfspherePair(planet.item);
        RotationAlongVelocitySide1(hsPair.hs1, velocity);
        RotationAlongVelocitySide2(hsPair.hs2, velocity);
        
        hsPair.hs1.transform.position = new Vector3(hsPair.hs1.transform.position.x, gameObj.transform.position.y, hsPair.hs1.transform.position.z);
        hsPair.hs2.transform.position = new Vector3(hsPair.hs2.transform.position.x, gameObj.transform.position.y, hsPair.hs2.transform.position.z);
        
        hsPair.hs1.GetComponent<Rigidbody>().velocity = velocity;
        hsPair.hs2.GetComponent<Rigidbody>().velocity = velocity;
        
        hsPair.hs1.transform.position = HalfspheresCoords(gameObj.transform.position, velocity, velocity.z > 0, true);
        hsPair.hs2.transform.position = HalfspheresCoords(gameObj.transform.position, velocity, velocity.z > 0, false);
        
        hsPair.hs1.transform.Rotate(-90, 0, 0);
        hsPair.hs2.transform.Rotate(-90, 0, 0);

        hsPair.hs1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        hsPair.hs2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

        gameObj.SetActive(false);
        return hsPair;
    }

    List<Planet> DeleteMarked(List<Planet> planets)
    {
        List<Planet> afterDeletion = new List<Planet>();
        foreach (Planet planet in planets)
        {
            if (!planet.deleteMark)
            {
                afterDeletion.Add(planet);
            }
        }
        return afterDeletion;
    }

    List<Halfsphere> DeleteMarked(List<Halfsphere> hss)
    {
        List<Halfsphere> afterDeletion = new List<Halfsphere>();
        foreach (Halfsphere hs in hss)
        {
            if (!hs.deleteMark)
            {
                afterDeletion.Add(hs);
            }
        }
        return afterDeletion;
    }

    void SwitchRendering(Planet planet)
    {
        if (planet.switchRendering.lastSwitchFrame == realFrameCount)
            return;

        planet.switchRendering.lastSwitchFrame = realFrameCount;
        planet.planet.GetComponent<MeshRenderer>().material.color = Color.red;

        if (planet.collisionChecker.GetComponent<SphereCollider>() != null)
        {
            Destroy(planet.collisionChecker.GetComponent<SphereCollider>());//.enabled = false;
            planet.switchRendering = new RenderSwitch(true, numberOfSwitches, dissapearThen);     // setare numar 'sclipiri' inainte de disparitie
        }
        else if (realFrameCount % framesBetweenSwitching != 0)
            return;
        
        if (planet.switchRendering.switchesLeft-- > 0) // the real switching   v
        {
            planet.planet.GetComponent<MeshRenderer>().enabled = !planet.planet.GetComponent<MeshRenderer>().enabled;
        }

        if (!dissapearThen && planet.switchRendering.switchesLeft == 0)
        {
            planet.planet.GetComponent<MeshRenderer>().enabled = true;
            planet.collisionChecker.AddComponent<SphereCollider>();
            planet.planet.GetComponent<MeshRenderer>().material.color = Color.white;
            planet.switchRendering.allowed = false;
        }
    }

    void SwitchRendering(FlickeringLine flickeringLine)
    {
        if (realFrameCount % framesBetweenSwitching != 0)
            return;

        flickeringLine.gO.GetComponent<LineRenderer>().enabled = !flickeringLine.gO.GetComponent<LineRenderer>().enabled;
        flickeringLine.switchesLeft--;
    }

    /// <summary>
    /// Motivul unmark-ului: vom folosi deleteMark-ul acum pentru a sterge planete din lista flickering. Prima oara se folosea doar pentru stergerea din memory.
    /// </summary>
    void UnmarkDeletion(List<Planet> planets)
    {
        foreach (Planet planet in planets)
        {
            planet.deleteMark = false;
        }
    }

    void Create(Vector3 position)
    {
        GameObject g1;
        g1 = Instantiate(gameObject, position, transform.rotation);
        g1.GetComponent<PathReader1>().enabled = false;
        g1.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        g1.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    void Create(Vector3 position, float scale, Color color)
    {
        GameObject g1;
        g1 = Instantiate(gameObject, position, transform.rotation);
        g1.GetComponent<PathReader1>().enabled = false;
        g1.transform.localScale = new Vector3(scale, scale, scale);
        g1.GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    void CreateN(Vector3 pos1, Vector3 pos2)
    {
        GameObject g1;
        g1 = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        g1.GetComponent<PathReader1>().enabled = false;
        g1.GetComponent<Renderer>().enabled = false;
        if (g1.GetComponent<LineRenderer>() == null)
        {
            g1.AddComponent<LineRenderer>();
        }
        g1.GetComponent<LineRenderer>().material.SetColor("_Color", Color.blue);
        g1.GetComponent<LineRenderer>().widthMultiplier = 0.04f;
        g1.GetComponent<LineRenderer>().positionCount = 2;
        g1.GetComponent<LineRenderer>().SetPosition(0, pos1);
        g1.GetComponent<LineRenderer>().SetPosition(1, pos2);
    }

    void CreateP(Vector3 pos1, Vector3 pos2)
    {
        GameObject g1;
        g1 = Instantiate(gameObject, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        g1.GetComponent<PathReader1>().enabled = false;
        g1.GetComponent<Renderer>().enabled = false;
        if (g1.GetComponent<LineRenderer>() == null)
        {
            g1.AddComponent<LineRenderer>();
        }
        g1.GetComponent<LineRenderer>().material.SetColor("_Color", Color.red);
        g1.GetComponent<LineRenderer>().widthMultiplier = 0.03f;
        g1.GetComponent<LineRenderer>().positionCount = 2;
        g1.GetComponent<LineRenderer>().SetPosition(0, pos1);
        g1.GetComponent<LineRenderer>().SetPosition(1, pos2);
    }

    void DeleteTrajs(List<GameObject> L)
    {
        foreach (GameObject l in L)
        {
            Destroy(l);
        }
        deleteTrajs = false;
    }

    /// <summary>
    /// After this, DeleteTrajs RECOMMENDED!
    /// </summary>
    List<Planet> DeleteOrbiting(List<Planet> planets)
    {
        foreach (Planet planet in planets)
        {
            Destroy(planet.planet);
            Destroy(planet.collisionChecker);
        }
        deleteOrbiting = false;
        return new List<Planet>(); ;
    }

    List<Halfsphere> DeleteFreeMotion(List<Halfsphere> freeMotion)
    {
        foreach (Halfsphere hs in freeMotion)
        {
            Destroy(hs.hs);
            Destroy(hs.collisionChecker);
        }
        deleteFreeMotion = false;
        return new List<Halfsphere>();
    }

    float FlatSlope(Vector3 p1, Vector3 p2)
    {
        return (p2.z - p1.z) / (p2.x - p1.x);
    }

    /// <summary>
    /// Arguments must be in consecutive order!
    /// </summary>
    float AngleBetweenPoints(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Angle(p1 - p2, p3 - p2);
    }

    List<Vector3> VectorToList(Vector3[] vector)
    {
        List<Vector3> list = new List<Vector3>();
        for (int idx = 0; idx < vector.Length; idx++)
        {
            list.Add(vector[idx]);
        }
        return list;
    }

    Vector3[] VectorAtHeight(Vector3[] markersV, float height)
    {
        Vector3[] markersW = (Vector3[])markersV.Clone();
        for (int i = 0; i < markersW.Length; i++)
        {
            Vector3 pos = markersW[i];
            pos = new Vector3(pos.x, height, pos.z);
            markersW[i] = pos;
        }

        return markersW;
    }

    public static List<Planet> memory, flickering; // flickering e NUMAI pentru dissapearThen = true, altfel planetele raman in memory!!!
    List<Halfsphere> motionFreePlanets;
    List<Vector3> markers;
    Vector3[] markersPV, markersV, markersXV;
    List<GameObject> trajsLst, planets;
    List<FlickeringLine> wrongTrajsLst;
    GameObject gO, planetObject, cursorPlanet;
    public float minimumCompletionAngle;
    public float hsInBetweenDistance;   // distanta dintre semisfere (ar fi trebuit sa fie proportionala cu dimensiunea planetei considerate, dar fiind egala cu 0, o las asa)
    public float smootheningFactor; // distance (relative to optimalDst) of tolerance between node k and the line which intersects the node k-1 and node k+1
    public int numberOfSwitches;
    public int framesBetweenSwitching;
    public bool dissapearThen;
    public int numberOfLineSwitches;
    int indexx, ordIdx;
    public static System.Random randomGen;
    private bool cursorPlanetTexAssigned;

    public static int realFrameCount;
    private const int INTEGRAL = -1;
    private const int IN_DESTRUCTION = 0;
    private const int DESTRUCTED = 1;

    public bool deleteOrbiting, deleteFreeMotion, deleteTrajs;

    int interp; // interpolation
    float optimalDst; // optimal distance between each consecutive 2 points on lines drawing

    private void Awake()
    {
        realFrameCount = 0;
    }

    void Start()
    {
        Inventory.initializeInventory();

        memory = new List<Planet>();
        motionFreePlanets = new List<Halfsphere>();
        flickering = new List<Planet>();
        trajsLst = new List<GameObject>();
        wrongTrajsLst = new List<FlickeringLine>();
        minimumCompletionAngle = 250; // toleranta pentru greseli la desen !
        interp = 2;
        optimalDst = 0.1f;
        smootheningFactor = 0.02f;
        hsInBetweenDistance = 0f;
        numberOfSwitches = 6;
        framesBetweenSwitching = 20;
        dissapearThen = false;
        numberOfLineSwitches = 5;
        indexx = 0;
        ordIdx = 0;
        randomGen = new System.Random();

        deleteOrbiting = deleteFreeMotion = deleteTrajs = false;

        planetObject = GameObject.Find("Planet");
        planetObject.SetActive(false);
        planetObject.transform.position = Vector3.up * -5;
        planetObject.AddComponent<Rigidbody>();
        planetObject.GetComponent<Rigidbody>().useGravity = false;

        //setBaseOpacity(-0.2f);
        gameObject.AddComponent<EventSystem>();

        // configuring planet which stays with the cursor:
        cursorPlanet = GameObject.Find("CursorPlanet");
        cursorPlanet.SetActive(true);
        cursorPlanet.GetComponent<SphereCollider>().enabled = false;

        RandomPlanetSelector.selectRandomPlanet();

        cursorPlanetTexAssigned = false;
    }

    void Update()
    {
        if (MainSceneUI.gameIsPaused())
        {
            foreach (var hs in motionFreePlanets)
            {
                hs.hs.GetComponent<FreezeOnPause>().Freeze();
            }

            return;
        } else
        {
            foreach (var hs in motionFreePlanets)
            {
                hs.hs.GetComponent<FreezeOnPause>().Unfreeze();
            }
        }


        // actiuni legate strict de CursorPlanet:
        if (Input.GetMouseButton(0))
        {
            cursorPlanet.GetComponent<MeshRenderer>().enabled = true;
        }
        else cursorPlanet.GetComponent<MeshRenderer>().enabled = false;

        if (!cursorPlanetTexAssigned)
        {
            cursorPlanet.transform.localScale = Vector3.one * (SceneParameters.currentPlanet.size / 10.0f);
            Texture planetTex = Resources.Load("PlanetSurfaces/Unit/" + SceneParameters.currentPlanet.name) as Texture;
            cursorPlanet.GetComponent<MeshRenderer>().material.mainTexture = planetTex;

            cursorPlanetTexAssigned = true;
        }
        //////
        


        if (Input.touchCount >= 0)  // touchCount se refera doar la touch-urile de pe touchScreen. Daca esti pe PC va fi mereu 0, chiar si la apasarea mouse-ului!
        {
            //if (Input.GetTouch(0).phase == TouchPhase.Began)
            if (Input.GetMouseButtonDown(0))
            {
                markers = new List<Vector3>();
            }
        }

        if (Input.touchCount >= 0)
        {
            //if (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved)
            if (Input.GetMouseButton(0)) // drawing
            {
                RaycastHit hit;
                Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);
                //Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit, 1000.0f) && hit.transform.tag == "ToRaycast")
                {
                    markers.Add(hit.point);

                    cursorPlanet.transform.position = hit.point;
                }
            }
        }

        if (Input.touchCount >= 0)
        {
            //if (Input.GetTouch(0).phase == TouchPhase.Ended)
            if (Input.GetMouseButtonUp(0))
            { // computing -- mai intai fac dispersia

                //SetTrajectoryHeight(markers, 0);  -- PENTRU 3D NU MAI E NEVOIE DE ASTA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Vector3[] intermediate = markers.ToArray();
                List<Vector3> markersList = new List<Vector3>();

                int point = 0, index = 0;
                while (index < intermediate.Length)                     // ..................point.................index....................
                    // acest while face dispersia si deci markersList e lista de puncte dispersate
                {
                    if ((intermediate[index] - intermediate[point]).magnitude < optimalDst)
                    {
                        index++;
                    }
                    else
                    {
                        markersList.Add(intermediate[point]);
                        point = index;
                    }
                }
                //INFO: markersList e markers dar cu dispersia aplicata !!

                // in urmatoarele 17 linii se calculeaza oposed, care reprezinta perechea de puncte cele mai departate unele de altele din traiectorie
                markersPV = markersList.ToArray();
                Oposed oposed = new Oposed()
                {
                    dst = 0
                };
                for (int i = 0; i < markersPV.Length; i++)
                {
                    for (int j = 0; j < markersPV.Length; j++)
                    {
                        if ((markersPV[i] - markersPV[j]).magnitude > oposed.dst)
                        {
                            oposed.dst = (markersPV[i] - markersPV[j]).magnitude;
                            oposed.from = i;
                            oposed.to = j;
                        }
                    }
                }

                //Debug.Log("markersPV[0] = " + markersPV[0].ToString() + " and markersPV[last] = " + markersPV[markersPV.Length - 1].ToString());

                // in urmatoarele 10 linii se va calcula realLength, adica cea pornind din poz initiala, mergand pana la punctul oposed.to, apoi continuand pana cand pozitia x devine mai mare ca pozitia x a poz initiale
                int realLength = oposed.to;
                while (realLength < markersPV.Length)
                {
                    if (markersPV[realLength].x > markersPV[0].x)
                    {
                        realLength++;
                    }
                    else break;
                }

                //Debug.Log("realLength = " + realLength.ToString() + " but original elngth was " + markersPV.Length.ToString());

                markersV = new Vector3[realLength + 1]; // markersV e traiectoria scurtata depinzand de pozitia x (pentru a nu evita traiectoriile esuate pe arc > 360gr)
                for (int idx = 0; idx < realLength; idx++)
                {
                    markersV[idx] = markersPV[idx];
                }
                markersV[realLength] = markersV[0];

                int firstLeftSide = oposed.to;
                while (firstLeftSide < realLength)
                {
                    if (markersV[firstLeftSide] == LineApproximation(markersV[firstLeftSide - 1], markersV[0], markersV[firstLeftSide], leftSide))
                    {
                        firstLeftSide++;
                    }
                    else break;
                }

                for (int idx = firstLeftSide; idx < markersV.Length; idx++)
                {
                    markersV[idx] = LineApproximation(markersV[firstLeftSide], markersV[0], markersV[idx], leftSide);
                }

                markersV = FlattenInnerCurves(VectorToList(markersV));

                AllowedShape shape = AllowToCloseShape(markersV);
                if (shape.allowed)
                {
                    markersV = shape.shape;

                    markersXV = CloseShape(markersV);
                    markersXV = PointsAtUniformDistance(VectorToList(markersXV)); // CU dispersie SI interp liniara

                    markersXV = SmoothenBounds(markersXV, smootheningFactor);
                    trajsLst.Add(ShowTrajectory(markersXV, Color.green));

                    // WE HAVE THE TRAJECTORY: MARKERSXV.
                    Inventory.takeItemOfType(SceneParameters.currentPlanet.name);

                    memory.Add(new Planet(
                        SceneParameters.selectCurrentPlanetAndGoToNext(),   // returneaza item-ul planetei curente si alege random item-ul pentru planeta urmatoare (practic FIX AICI se decide care va fi planeta urmatoare)
                        Instantiate(planetObject), 
                        markersXV, 
                        0, 
                        indexx++, 
                        realFrameCount, 
                        ordIdx++
                    ));
                    // s-a decis care va fi planeta urmatoare, deci vom dez-assigna textura, pentru a o seta in frame-ul imediat urmator (la cam ~inceputul functiei Update()):
                    cursorPlanetTexAssigned = false;

                    XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
                    FileStream stream = new FileStream(Application.dataPath + "/xml_stuff/exported_path.xml", FileMode.Create);
                    serializer.Serialize(stream, markersXV);
                    stream.Close();
                }
                else
                {
                    wrongTrajsLst.Add(new FlickeringLine(numberOfLineSwitches, framesBetweenSwitching, ShowTrajectory(markers.ToArray(), Color.red)));
                }
            }
        }

        //////////////////////////////////////////////
        foreach (FlickeringLine line in wrongTrajsLst)
        {
            SwitchRendering(line);
        }
        wrongTrajsLst = wrongTrajsLst.Where(list => list.switchesLeft != 0).ToList();
        //////////////////////////////////////////////


        ////// emisferele care au atins timeFrame-ul de startDestruction vor incepe sa se dezintegreze si li se va sterge collider-ul //
        foreach (Halfsphere hs in motionFreePlanets)
        {
            if (hs.status() == IN_DESTRUCTION && hs.fade > 0f)
                hs.fade -= hs.fadeUnity;
            else if (hs.status() == DESTRUCTED)
            {
                Destroy(hs.hs);
                Destroy(hs.collisionChecker);
                hs.deleteMark = true;
            }
            hs.hs.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Fade", hs.fade);
        }

        motionFreePlanets = DeleteMarked(motionFreePlanets);
        //////////////////////////////////////////////

        // updatarea pozitiei planetelor (in fiecare frame fiecare planeta din memory se muta la punctul de pe traiectorie cu indexul urmator)
        for (int idx = 0; idx < memory.Count; idx++)
        {
            memory[idx].planet.transform.position = memory[idx].trajectory[memory[idx].rotationIndex];
            memory[idx].collisionChecker.transform.position = memory[idx].trajectory[NextIndex(memory[idx])];
            memory[idx].rotationIndex = NextIndex(memory[idx]);
        }

        List<int> collisionBetweenOrbiting = new List<int>();
        for (int idx = 0; idx < memory.Count; idx++)
        {
            collisionBetweenOrbiting = collisionBetweenOrbiting.Union(CollisionListXFramesLater(1, memory, motionFreePlanets, idx, memory[idx].rotationIndex)).ToList();
        }
        
        foreach (int idx in collisionBetweenOrbiting)
        {
            memory[idx].planet.GetComponent<MeshRenderer>().material.color = Color.blue;
            memory[idx].deleteMark = true;
            memory[idx].collisionParticles = Disintegrate(memory[idx], MotionToVelocity(memory[idx].trajectory, memory[idx].rotationIndex));
            memory[idx].collisionChecker.SetActive(false); // checker-ul trebuie de asemenea sa dispara
        }
        
        foreach (Planet planetObj in memory)
        {
            if (planetObj.deleteMark && !planetObj.switchRendering.allowed)
            {
                planetObj.planet.GetComponent<Rigidbody>().velocity = MotionToVelocity(planetObj.trajectory, planetObj.rotationIndex); // aceeasi viteza ca mai inainte, dar de data asta dreapta si continua, nu discreta, generata de inertie, atunci cand planeta paraseste orbita
                Halfsphere hs1 = new Halfsphere(planetObj.collisionParticles.hs1, planetObj.item, ordIdx++);
                Halfsphere hs2 = new Halfsphere(planetObj.collisionParticles.hs2, planetObj.item, ordIdx++);
                // vom seta pozitia initiala a planetelor fantoma. NU setez velocity, deoarece e bine ca ele sa se deplaseze discret, updatandu-si pozitia in functie de pozitia planetelor nefantoma de care apartin.
                hs1.collisionChecker.transform.position = hs1.hs.transform.position + hs1.hs.GetComponent<Rigidbody>().velocity * Time.deltaTime;
                hs2.collisionChecker.transform.position = hs2.hs.transform.position + hs2.hs.GetComponent<Rigidbody>().velocity * Time.deltaTime;

                motionFreePlanets.Add(hs1);
                motionFreePlanets.Add(hs2);
                planetObj.planet.SetActive(false);
            }
            if (planetObj.switchRendering.allowed) // daca asa, inseamna ca s-a allowat chiar in timpul frame-ului asta, deoarece in frame-ul urmator nu se va mai gasi in memory, ci doar in flockering (logic de ce: pentru ca vom pune acum deleteMark-ul, iar inainte de finalul acestui frame, se va sterge din memory, dar nu isi va da destroy (inca), ci isi va da abia cand iese din flickering)
            {
                if (planetObj.switchRendering.dissapearThen)
                {
                    planetObj.deleteMark = true;
                    flickering.Add(planetObj);
                } else // continua miscarea 'clipind' si fara collider
                {
                    SwitchRendering(planetObj);
                }
            }
        }

        memory = DeleteMarked(memory);

        // chestia asta reprezinta inregistrarea + dezintegrarea pentru ciocnirile intre planete si semisfere si NU MAI ARE ROST
        // deoarece cu asta s-a ocupat si loop-ul de mai sus!! (de ciocniri planeta-planeta si planeta-semisfera)
        List<Halfsphere> supply = new List<Halfsphere>();
        
        // IN LOOP-UL ASTA NU SE INTRA NICIODATA!!
        foreach (Halfsphere freeMotionPlanet in motionFreePlanets)
            {
            foreach (Planet orbitingPlanet in memory)
            {
                if (orbitingPlanet.switchRendering.allowed)
                {
                    continue;
                }
                if (orbitingPlanet.collisionChecker.GetComponent<Checker>().collidedWithHalfsphere)
                //if (freeMotionPlanet.collisionChecker.GetComponent<Checker>().collidedWithPlanet) // (14.03.2020)
                {
                    HalfspherePair pair = Disintegrate(orbitingPlanet, MotionToVelocity(orbitingPlanet.trajectory, orbitingPlanet.rotationIndex));
                    Halfsphere hs1 = new Halfsphere(pair.hs1, orbitingPlanet.item, ordIdx++);
                    Halfsphere hs2 = new Halfsphere(pair.hs2, orbitingPlanet.item, ordIdx++);
                    // ne trebuie pozitia, pentru a verifica in bucla urmatoare, in if, variabila collided (in scriptul celalalt se va detecta sau nu ciocnirea, deci trebuie neaparat pozitia curenta a fantomei inca de acum).
                    hs1.collisionChecker.transform.position = hs1.hs.transform.position + hs1.hs.GetComponent<Rigidbody>().velocity * Time.deltaTime;
                    hs2.collisionChecker.transform.position = hs2.hs.transform.position + hs2.hs.GetComponent<Rigidbody>().velocity * Time.deltaTime;
                    supply.Add(hs1);
                    supply.Add(hs2); // in motionFreePlanets oricum nu conteaza ceilalti parametri din clasa Planet in afara de gameobject
                    orbitingPlanet.collisionChecker.gameObject.SetActive(false);
                    orbitingPlanet.deleteMark = true;
                    break;
                }
            }
            motionFreePlanets.Union(supply);
            // supply = new List<Halfsphere>(); // (14.03.2020) cred ca ar trebui sa decomentez, deoarece degeaba se face Union mereu cu aceleasi obiecte din supply despre care se stie ca se afla deja in motionFreePlanets. Asadar, supply ar avea la fiecare iteratie (Union) doar 0 sau 2 elemente.
        }
        
        foreach (Halfsphere freeMotionPlanet in motionFreePlanets) // for-ul care updateaza pozitia semisferelor fantoma, deoarece nu e o idee buna ca deplasarea lor sa se faca prin velocity, ci e bine deplasare discreta. (motiv: fantomele au trigger-ul activat, deci la o eventuala coliziune, ele nu isi vor schimba directia, si deci se vor indeparta de planetele nefantoma de care apartin)
        {
            freeMotionPlanet.collisionChecker.transform.position = freeMotionPlanet.hs.transform.position + freeMotionPlanet.hs.GetComponent<Rigidbody>().velocity * Time.deltaTime;
            freeMotionPlanet.collisionChecker.transform.rotation = freeMotionPlanet.hs.transform.rotation;
        } // stai putin, position-ul nu s-a calculat mai sus? (cu 16-15 linii mai sus), ca oricum numai semisferele sunt motionFreePlanets
          // rotation-ul intr-adevar nu se calculase

        UnmarkDeletion(flickering); // could be a bit more efficient, instead of unmarking all contents every frame, can only the last be unmarked, cuz the rest are already
        foreach (Planet planet in flickering)
        {
            if (planet.switchRendering.switchesLeft == 0)
            {
                planet.deleteMark = true;
                planet.planet.SetActive(false);
            }
            SwitchRendering(planet);
        }

        flickering = DeleteMarked(flickering);

        if (deleteOrbiting) memory = DeleteOrbiting(memory);
        if (deleteFreeMotion) motionFreePlanets = DeleteFreeMotion(motionFreePlanets);
        if (deleteTrajs) DeleteTrajs(trajsLst);

        realFrameCount++;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (markersXV != null)
        {
            foreach (Vector3 point in markersXV)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}