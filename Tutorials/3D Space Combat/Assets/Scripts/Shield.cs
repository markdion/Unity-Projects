﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Shield : MonoBehaviour {

    public float rechargeRate;
    public float waitBeforeRecharge = 5f;
    public float maxCharge;
    public UIProgressBarController healthBar;

    private float _charge;
    private float _wait;

    void Start ()
    {
        _charge = maxCharge;
	}
	
	void Update ()
    {
	    if (_charge < maxCharge && _wait < Time.time)
        {
            _charge += rechargeRate * Time.deltaTime;
        }
	}

    public float Charge
    {
        get
        {
            return _charge;
        }
    }

    public void Damage(DamageInfo damageInfo)
    {
        _charge -= damageInfo.Damage;

        if (healthBar != null)
        {
            healthBar.fillAmount = (float)_charge / (float)maxCharge;
        }

        _wait = Time.time + waitBeforeRecharge;
    }
}