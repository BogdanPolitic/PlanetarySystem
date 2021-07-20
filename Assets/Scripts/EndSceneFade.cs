using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneFade : MonoBehaviour
{
    GameObject cameraFade, cameraFadeDummy;
    Vector3 initialPosition;
    static bool FADE_JUST_STARTED;
    static bool IN_FADING_PROCESS;
    static float fadingTimelapse;
    public static float FADE_DURATION = 0.5f;
    void Start()
    {
        cameraFade = gameObject;
        FADE_JUST_STARTED = false;
        IN_FADING_PROCESS = false;

        initialPosition = cameraFade.transform.position;
        cameraFadeDummy = GameObject.Find("CameraFadeDummy");
    }

    void Update()
    {
        //Debug.Log("camera fade is at " + cameraFade.transform.position);
        if (FADE_JUST_STARTED)
        {
            FADE_JUST_STARTED = false;
            IN_FADING_PROCESS = true;
        }

        if (IN_FADING_PROCESS)
        {
            if (fadingTimelapse > FADE_DURATION)
            {
                SceneManager.LoadScene(0);
                return;
            }

            fadingTimelapse += Time.deltaTime;
            cameraFade.transform.position = new Vector3(
                (fadingTimelapse - FADE_DURATION) / FADE_DURATION,
                cameraFade.transform.position.y,
                cameraFade.transform.position.z
                );

            float translationProcent = (fadingTimelapse - FADE_DURATION) / FADE_DURATION;
            cameraFade.transform.position = (cameraFadeDummy.transform.position - cameraFade.transform.position) * translationProcent;
        }
    }

    public void startFadingAndGoToMenu()
    {
        if (FADE_JUST_STARTED || IN_FADING_PROCESS)
            return;

        FADE_JUST_STARTED = true;
        fadingTimelapse = 0;
    }
}
