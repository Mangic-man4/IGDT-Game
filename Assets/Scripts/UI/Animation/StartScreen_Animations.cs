using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StartScreen_Animations : MonoBehaviour
{
    public Animator MenuAnimator;
    public GameObject PromptText;

    private static bool playedAnimation = false;

    private void Awake()
    {
        if (playedAnimation)
        {
            PromptText.SetActive(false);
            GetComponent<Animator>().enabled = false;
        }
    }

    private void Start()
    {
        if (!playedAnimation)
        {
            StartCoroutine(WaitForSpace());
        }
    }

    IEnumerator WaitForSpace()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        PromptText.SetActive(false);
        MenuAnimator.SetBool("canIntro", true);
        playedAnimation = true;
    }
}