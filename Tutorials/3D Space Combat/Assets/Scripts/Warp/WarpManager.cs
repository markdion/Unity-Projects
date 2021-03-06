﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpManager : MonoBehaviour {

    public WarpUI warpUI;
    public WarpEffects effects;
    public WarpPlayer player;
    public WarpCamera warpCamera;
    public ThrusterGroup thrusters;

    public Transform cameraTarget;

    private Action<AbstractGoTween> _onComplete;

    public Vector3 Destination { get; set; }

    public void Warp()
    {
        warpUI.ChargeComplete += Engage;
        warpUI.Begin();
    }

    public void Engage()
    {
        effects.EnterWarp();
        thrusters.SetMaxPower();
        player.SetOnCompleteHandler(delegate (AbstractGoTween t)
        {
            //Go.to(warpCamera.transform, 0.5f, new GoTweenConfig().shake(new Vector3(1, 1, 1), GoShakeType.Position, 2));
            GameManager.instance.CameraController.ShakeCamera(0.4f, 10, 0.3f);
            effects.ExitWarp();
            _onComplete.Invoke(t);
        });
        player.SetOnUpdateHandler(delegate (AbstractGoTween t)
        {
            if (t.totalDuration - t.totalElapsedTime < 0.8f) effects.EndStars();
        });
        player.Warp(Destination);
        Debug.Log("Warp called");

        var camOffset = CalculatePositionOffset(cameraTarget.position, player.transform.position);
        warpCamera.Warp(Destination + camOffset);
    }

    public void SetOnCompleteHandler(Action<AbstractGoTween> handler)
    {
        _onComplete = handler;
    }

    private Vector3 CalculatePositionOffset(Vector3 offsetFromTarget, Vector3 target)
    {
        var x = offsetFromTarget.x - target.x;
        var y = offsetFromTarget.y - target.y;
        var z = offsetFromTarget.z - target.z;
        return new Vector3(x, y, z);
    }
}
