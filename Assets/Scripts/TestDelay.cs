using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDelay : MonoBehaviour
{
    [SerializeField] Text origText;
    [SerializeField] Text thisText;
    [SerializeField] Animator animator;
    private float delay = 0.1f;

    IEnumerator TakeSize(Vector3 actualSize, float frameC)
    {
        yield return new WaitForSeconds(delay);
        thisText.rectTransform.localScale = actualSize;
    }

    void Update()
    {
        StartCoroutine(TakeSize(origText.rectTransform.localScale, Time.frameCount));
    }
}
