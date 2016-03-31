﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float idleSpeed = 100.0f;
    public float moveSpeed = 200.0f;
    public float strafeSpeed = 100.0f;
    public float combatBoostSpeed = 400.0f;
    public float boostSpeed = 1000.0f;
    public float lookSpeed = 0.1f;
    public int verticalLookLimit = 60;
    public float tilt = 5.0f;
    public RadialBlur radialBlur;
    public InverseVignette vignette;

    private Rigidbody rb;
    private WarpDrive warpDrive;
    private bool movementLocked = false;
    private State currentState = State.Default;
    
    private enum State
    {
        Default,
        WarpStandby,
        Docked
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        warpDrive = GetComponent<WarpDrive>();
    }

    void Update()
    {
        if(currentState == State.Docked)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Undock();
            }
            else
            {
                return;
            }
        }

        // Lock onto warp target
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Cancel lock
            if(currentState == State.WarpStandby)
            {
                LockMovement(false);
                currentState = State.Default;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                WarpTarget target = hit.collider.gameObject.GetComponent<WarpTarget>();
                if(target != null)
                {
                    LockMovement(true);
                    warpDrive.SetTarget(target.targetTransform.position);
                    StartCoroutine(RotateTowards(target.targetTransform.position));
                    currentState = State.WarpStandby;
                }
            }
        }

        // Engage warp
        if(currentState == State.WarpStandby)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                warpDrive.Engage();
            }

            if (warpDrive.State == Enums.WarpDriveState.waitingForCommand)
            {
                warpDrive.PowerDown();
                LockMovement(false);
                currentState = State.Default;
            }
        }

        // Check for enemies that are near
        TargetableObject[] objects = GameObject.FindObjectsOfType(typeof(TargetableObject)) as TargetableObject[];

        if(objects.Length <= 0)
        {
            GameManager.instance.isInCombat = false;
        }
        else
        {
            foreach (TargetableObject obj in objects)
            {
                if (Vector3.Distance(obj.transform.position, transform.position) < 500)
                {
                    GameManager.instance.isInCombat = true;
                    break;
                }
                else
                {
                    GameManager.instance.isInCombat = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!movementLocked)
        {
            float inputVertical = Input.GetAxis("Vertical");
            float inputHorizontal = Input.GetAxis("Horizontal");
            Vector3 mousePosition = Input.mousePosition;

            #region Velocity Logic
            // Add base force
            rb.AddForce(transform.forward * idleSpeed);

            // Add forward or backward force
            if (inputVertical > 0)
            {
                rb.AddForce(transform.forward * moveSpeed * inputVertical);

                // Add boost speed force
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    GameManager.instance.isShootingEnabled = false;
                    //radialBlur.TurnOn();
                    //vignette.TurnOn();
                    if (GameManager.instance.isInCombat)
                    {
                        rb.AddForce(transform.forward * combatBoostSpeed);
                    }
                    else
                    {
                        rb.AddForce(transform.forward * boostSpeed);
                    }
                }
                else
                {
                    GameManager.instance.isShootingEnabled = true;
                    //radialBlur.TurnOff();
                    //vignette.TurnOff();
                }
            }
            else
            {
                rb.AddForce(transform.forward * idleSpeed * inputVertical);
            }

            rb.AddForce(transform.right * strafeSpeed * inputHorizontal);
            #endregion

            #region Rotation Logic
            // Get the direction the mouse is pointing
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Quaternion rayDirection = Quaternion.LookRotation(ray.direction);
            //rb.AddTorque(ray.direction.x * lookSpeed, ray.direction.y * lookSpeed, 0f);

            // Clamp the direction on the x-axis (up/down) to prevent spiraling camera
            Vector3 directionEuler = rayDirection.eulerAngles;
            Quaternion modifiedDirection = Quaternion.Euler(Mathf.Clamp(directionEuler.x > 180 ? directionEuler.x - 360 : directionEuler.x, -verticalLookLimit, verticalLookLimit), directionEuler.y, directionEuler.z);

            // Rotate along the z-axis to counteract x movement
            directionEuler = modifiedDirection.eulerAngles;
            modifiedDirection = Quaternion.Euler(directionEuler.x, directionEuler.y, transform.InverseTransformDirection(rb.velocity).x * tilt);

            // Rotate back to 0 if it has been knocked out of rotation
            float distanceFromCenterX = Mathf.Abs((Screen.width / 2) - mousePosition.x);
            float distanceFromCenterY = Mathf.Abs((Screen.height / 2) - mousePosition.y);

            if (inputHorizontal == 0 && distanceFromCenterX < 32 && distanceFromCenterY < 32)
            {
                modifiedDirection = Quaternion.Euler(modifiedDirection.eulerAngles.x, modifiedDirection.eulerAngles.y, 0f);
            }

            // Apply the rotation
            rb.rotation = Quaternion.Slerp(rb.rotation, modifiedDirection, lookSpeed);
            //rb.AddTorque(modifiedDirection.eulerAngles * lookSpeed);
            #endregion
        }
    }

    public void LockMovement(bool setting)
    {
        movementLocked = setting;
    }

    public void Dock(Transform dockingTransform)
    {
        StartCoroutine(PerformDock(dockingTransform, 3f));
    }

    public void Undock()
    {
        StartCoroutine(PerformUndock(3f));
    }

    IEnumerator RotateTowards(Vector3 lookAtPosition)
    {
        //Vector3 direction = (lookAtPosition - transform.position).normalized;
        //Quaternion directionQuaternion = Quaternion.Euler(direction.x, direction.y, direction.z);
        Quaternion direction = Quaternion.LookRotation(lookAtPosition);
        while (Quaternion.Angle(rb.rotation, direction) > 0.1)
        {
            //print(string.Format("rotation: {0}", rb.rotation));
            rb.rotation = Quaternion.Slerp(rb.rotation, direction, lookSpeed);
        }
        yield return null;
    }

    IEnumerator PerformDock(Transform dockingTransform, float time)
    {
        currentState = State.Docked;
        LockMovement(true);
        GameManager.instance.isShootingEnabled = false;
        GameManager.instance.isCursorVisible = false;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 midPosition = dockingTransform.position + new Vector3(0f, 10f, 0f);
        Vector3 endPosition = dockingTransform.position;
        Quaternion endRotation = dockingTransform.rotation;
        float timeSinceStarted = 0f;
        float percentageComplete = 0f;
        float startTime = Time.time;

        while (transform.position != midPosition && transform.rotation != endRotation)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / time;
            transform.position = Vector3.Lerp(startPosition, midPosition, percentageComplete);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, percentageComplete);
            yield return null;
        }

        startPosition = transform.position;
        timeSinceStarted = 0f;
        percentageComplete = 0f;
        startTime = Time.time;

        while(transform.position != endPosition)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / time;
            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
            yield return null;
        }
    }

    IEnumerator PerformUndock(float time)
    {
        Vector3 startPosition = transform.position;
        Vector3 midPosition = startPosition + new Vector3(0f, 10f, 0f);
        Vector3 endPosition = midPosition + (transform.forward * 50f) + (transform.up * 5f);
        float timeSinceStarted = 0f;
        float percentageComplete = 0f;
        float startTime = Time.time;

        while (transform.position != midPosition)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / time;
            transform.position = Vector3.Lerp(startPosition, midPosition, percentageComplete);
            yield return null;
        }

        startPosition = transform.position;
        timeSinceStarted = 0f;
        percentageComplete = 0f;
        startTime = Time.time;

        while (transform.position != endPosition)
        {
            timeSinceStarted = Time.time - startTime;
            percentageComplete = timeSinceStarted / time;
            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);
            yield return null;
        }
        currentState = State.Default;
        LockMovement(false);
        GameManager.instance.isShootingEnabled = true;
        GameManager.instance.isCursorVisible = true;
    }
}
