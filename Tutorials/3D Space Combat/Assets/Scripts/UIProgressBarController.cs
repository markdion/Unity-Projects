﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIProgressBarController : MonoBehaviour {

    public Image barFullImage;
    public Image barEmptyImage;

    public float fillAmount
    {
        get
        {
            return barFullImage.fillAmount;
        }
        set
        {
            barFullImage.fillAmount = value;
        }
    }

    public Vector2 anchoredPosition
    {
        get
        {
            return barEmptyImage.rectTransform.anchoredPosition;
        }
        set
        {
            barEmptyImage.rectTransform.anchoredPosition = value;
            barFullImage.rectTransform.anchoredPosition = value;
        }
    }
}
