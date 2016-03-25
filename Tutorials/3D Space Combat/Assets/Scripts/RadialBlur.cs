﻿using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(Camera))]
public class RadialBlur : MonoBehaviour
{
    public Shader rbShader;

    public float blendAmount = 0.5f;
    public int samples = 8;

    private Material rbMaterial = null;
    private bool isOn = false;

    private Material GetMaterial()
    {
        if (rbMaterial == null)
        {
            rbMaterial = new Material(rbShader);
            rbMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        return rbMaterial;
    }

    void Start()
    {
        if (rbShader == null)
        {
            Debug.LogError("shader missing!", this);
        }
    }

    public void TurnOn()
    {
        isOn = true;
    }

    public void TurnOff()
    {
        isOn = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (isOn)
        {
            GetMaterial().SetVector("_Center", new Vector4(0.5f, 0.5f, 0f, 0f));
            GetMaterial().SetFloat("_BlendAmount", blendAmount);
            GetMaterial().SetInt("_Samples", samples);
            Graphics.Blit(source, dest, GetMaterial());
        }
        else
        {
            Graphics.Blit(source, dest);
        }
    }
}