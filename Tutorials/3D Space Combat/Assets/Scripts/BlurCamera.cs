﻿using UnityEngine;
using System.Collections;

public class BlurCamera : MonoBehaviour {

    [SerializeField]
    private Canvas parent;
    [SerializeField]
    private RectTransform referenceRect;

	void Start ()
    {
        Rect position = GetScreenRect(referenceRect, parent);
        float width = referenceRect.rect.width;
        float height = referenceRect.rect.height;

        gameObject.GetComponent<Camera>().pixelRect = position;
    }

    private Rect GetScreenRect(RectTransform rectTransform, Canvas canvas)
    {
        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];

        rectTransform.GetWorldCorners(corners);

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }

        screenCorners[0].y = screenCorners[0].y - referenceRect.rect.height;
        screenCorners[1].y = screenCorners[1].y - referenceRect.rect.height;

        return new Rect(screenCorners[0], new Vector2(referenceRect.rect.width, referenceRect.rect.height));
    }
}
