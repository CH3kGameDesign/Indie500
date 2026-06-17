/*
MESSAGE FROM CREATOR: This script was coded by Mena. You can use it in your games either these are commercial or
personal projects. You can even add or remove functions as you wish. However, you cannot sell copies of this
script by itself, since it is originally distributed as a free product.
I wish you the best for your project. Good luck!

P.S: If you need more cars, you can check my other vehicle assets on the Unity Asset Store, perhaps you could find
something useful for your game. Best regards, Mena.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MotorbikeController : PrometeoCarController
{
    public Transform T_uprightPivot;
    public float F_tiltMultiplier = -2;
    
    protected override void Update()
    {
        base.Update();
        Tilt();
    }
    void Tilt()
    {
        T_uprightPivot.localEulerAngles = new Vector3(0,0,base.frontLeftCollider.steerAngle* F_tiltMultiplier);
    }
}
