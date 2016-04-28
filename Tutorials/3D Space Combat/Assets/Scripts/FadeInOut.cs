﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class FadeInOut : MonoBehaviour {

    public static FadeInOut instance;

    private Animator anim;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void LoadLevel(string name)
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadLevelCoroutine(name));
    }

    public void LoadLevel(int buildIndex)
    {
        StartCoroutine(LoadLevelCoroutine(buildIndex));
    }

    IEnumerator LoadLevelCoroutine(string name)
    {
        anim.Play("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(name);
    }

    IEnumerator LoadLevelCoroutine(int buildIndex)
    {
        anim.Play("FadeOut");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(buildIndex);
    }

    IEnumerator DisableAfterFadeIn()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}