using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMeteors : MonoBehaviour
{
    class Meteor
    {
        public int lineIdx;
        public int columnIdx;
        public int heightIdx;
        public bool isMeteor;
        public float size;
        public Vector3 displacement;
        public Quaternion initialRotation;
        public Quaternion rotationPerSec;
        public int type;                       // type range in 1 ... 5 (each for a different prefab)
        public GameObject obj;

        public Meteor() { }

        public Meteor(int lineIdx, int columnIdx, int heightIdx, bool isMeteor, float size, Vector3 displacement, Quaternion initialRotation, Quaternion rotationPerSec, int type)
        {
            this.lineIdx = lineIdx;
            this.columnIdx = columnIdx;
            this.heightIdx = heightIdx;
            this.isMeteor = isMeteor;
            this.size = size;
            this.displacement = displacement;
            this.initialRotation = initialRotation;
            this.rotationPerSec = rotationPerSec;
            this.type = type;
        }
    }

    public static float R = 40.0f;      // se modifica la LoadSpecification
    public static float r = 30.0f;      // se modifica la LoadSpecification
    public static float height = 10.0f; // se modifica la LoadSpecification

    const int linesNumber = 12;     // numar de linii intr-un nivel (pe orizontala)
    const int levelsNumber = 12;    // numar de niveluri (etaje) (pe verticala)
    const float xlimInf = 0.7f;
    const float xlimSup = 2.7f;
    const float ylimInf = 0.9f;
    const float ylimSup = 2.5f;
    const float maxDisplacement = 2.0f;

    LinkedList<LinkedList<LinkedList<Meteor>>> meteors;
    LinkedList<LinkedList<Meteor>> currentLevel;
    LinkedList<Meteor> currentLine;

    float getRank(Vector3 position)
    {
        float magnitude = (position - Vector3.zero).magnitude;
        float horizontalCentralPeak = (R + r) / 2;
        float halfLineWidth = (R - r) / 2;
        float horizontalCentralRank = 1f - Mathf.Abs(horizontalCentralPeak - magnitude) / halfLineWidth;
        float verticalCentralRank = Mathf.Abs(position.y - height) / height;

        return horizontalCentralRank * 0.5f     // pondere de 50%
                + verticalCentralRank * 0.5f;   // pondere de 50%
    }

    void Start()
    {
        RNG.init();

        meteors = new LinkedList<LinkedList<LinkedList<Meteor>>>();
        float step = (2 * R) / linesNumber;


        float heightCompleted = -height;
        int heightIdx = 0;

        while (heightCompleted < height)
        {
            currentLevel = new LinkedList<LinkedList<Meteor>>();

            for (float lineIdx = 0.0f; lineIdx < linesNumber; lineIdx++)
            {
                float lineLevel = (lineIdx + 0.5f) * step;
                currentLine = new LinkedList<Meteor>();
                float length = 2 * Mathf.Sqrt(Mathf.Pow(R, 2) - Mathf.Pow(R - lineLevel, 2));
                float lengthCompleted = 0.0f;
                int columnIdx = 0;
                bool mustBeMeteor = true;

                while (lengthCompleted < length)
                {
                    float lineStep = mustBeMeteor ?
                                        RNG.getRandomFloat(xlimInf, xlimSup)
                                        : RNG.getRandomFloat(ylimInf, ylimSup);

                    if (Mathf.Pow(lengthCompleted - length / 2, 2)
                        + Mathf.Pow(R - lineLevel, 2) <= Mathf.Pow(r, 2))
                    {
                        lengthCompleted += lineStep;
                        continue;
                    }


                    Meteor meteor = new Meteor();
                    meteor.lineIdx = (int)lineIdx;
                    meteor.columnIdx = columnIdx;
                    meteor.heightIdx = heightIdx;
                    meteor.isMeteor = mustBeMeteor;

                    meteor.size = lineStep;

                    meteor.displacement = RNG.getRandomVector3(-maxDisplacement, maxDisplacement);
                    meteor.initialRotation = Quaternion.identity;
                    meteor.rotationPerSec = Quaternion.identity;
                    meteor.type = RNG.getRandomInt(1, 5);

                    if (mustBeMeteor)
                        meteor.obj = Instantiate(GameObject.Find("Asteroid_" + meteor.type));
                    else
                        meteor.obj = Instantiate(GameObject.Find("Space"));

                    meteor.obj.transform.position = new Vector3(lengthCompleted - length / 2
                                                                , heightCompleted + meteor.displacement.y
                                                                , R - lineLevel + meteor.displacement.x);


                    if (mustBeMeteor)
                        meteor.obj.transform.localScale = RNG.getRandomVector3(xlimInf, xlimSup) * 120f * getRank(meteor.obj.transform.position);
                    else
                        meteor.obj.transform.localScale = RNG.getRandomVector3(ylimInf, ylimSup);

                    meteor.obj.transform.rotation = Random.rotation;

                    currentLine.AddLast(meteor);
                    columnIdx++;
                    lengthCompleted += lineStep;
                    mustBeMeteor = !mustBeMeteor;
                }

                currentLevel.AddLast(currentLine);
            }

            heightCompleted += RNG.getRandomFloat(1.6f, 2.4f);
            meteors.AddLast(currentLevel);
            heightIdx++;
        }
    }

    void Update()
    {
        if (MainSceneUI.gameIsPaused())
            return;

        foreach (var currentLevel in meteors)
        {
            foreach (var currentLine in currentLevel)
            {
                foreach (var meteor in currentLine)
                {
                    meteor.obj.transform.Rotate(
                                                meteor.obj.transform.rotation.eulerAngles,
                                                10 * Time.deltaTime
                                            );
                    meteor.obj.transform.RotateAround(
                                                Vector3.zero,
                                                new Vector3(0, 1, 0.2f),
                                                5 * Time.deltaTime
                                            );
                }
            }
        }
    }
}
