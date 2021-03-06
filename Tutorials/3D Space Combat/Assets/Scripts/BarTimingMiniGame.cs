﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarTimingMiniGame : MonoBehaviour {

    [SerializeField]
    private float perfectPosition;
    [SerializeField]
    private float perfectRange;
    [SerializeField]
    private float acceptedRange;
    [SerializeField]
    private Image cursor;
    [SerializeField]
    private Image bar;
    [SerializeField]
    private int timeInSeconds = 5;

    public delegate void MiniGameResultDelegate(Enums.MiniGameResult result);
    public event MiniGameResultDelegate ResultReady;

    private float speed;
    private bool started = false;

	void Start ()
    {
        cursor.canvasRenderer.SetAlpha(0f);
        bar.canvasRenderer.SetAlpha(0f);
        cursor.rectTransform.anchoredPosition = new Vector2(bar.rectTransform.rect.xMin, 0f);
        speed = bar.rectTransform.rect.width / (Mathf.Pow(2, timeInSeconds) - 1);
	}
	
	void Update ()
    {
        if (started)
        {
            if (cursor.rectTransform.anchoredPosition.x < bar.rectTransform.rect.xMax)
            {
                speed += speed * Time.deltaTime;
                cursor.rectTransform.anchoredPosition += new Vector2(speed, 0f);
            }
            else
            {
                Close();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float cursorPositionX = cursor.rectTransform.anchoredPosition.x;
                if (cursorPositionX > perfectPosition - perfectRange && cursorPositionX < perfectPosition + perfectRange)
                {
                    // Perfect, skip entire countdown
                    if(ResultReady != null)
                    {
                        ResultReady(Enums.MiniGameResult.perfect);
                    }
                }
                else if (cursorPositionX > perfectPosition - acceptedRange && cursorPositionX < perfectPosition + acceptedRange)
                {
                    // Good, reduce countdown
                    if (ResultReady != null)
                    {
                        ResultReady(Enums.MiniGameResult.good);
                    }
                }
                Close();
            }
        }
	}

    public void Close()
    {
        bar.CrossFadeAlpha(0f, 0.25f, false);
        cursor.CrossFadeAlpha(0f, 0.25f, false);
        started = false;
    }

    public void StartMiniGame()
    {
        if (!started)
        {
            Reset();
            bar.CrossFadeAlpha(1f, 0.12f, false);
            cursor.CrossFadeAlpha(1f, 0.12f, false);
            started = true;
        }
    }

    private void Reset()
    {
        cursor.rectTransform.anchoredPosition = new Vector2(bar.rectTransform.rect.xMin, 0f);
        speed = bar.rectTransform.rect.width / (Mathf.Pow(2, timeInSeconds) - 1);
    }
}
