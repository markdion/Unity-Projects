﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Rigidbody))]
public class AINavigator : MonoBehaviour {

    public Vector2 forwardSpeed;
    public float rotateSpeed = 1f;
    public float avoidanceSpeed;
    public float avoidanceSpread = 5f;
    public float avoidanceRange = 400f;

    public event System.EventHandler DestinationReached;

    private Rigidbody _rb;
    private NpcType _npcType;
    private bool _isActive = false;
    private float _forwardSpeed;
    private GameObject _destinationGO;
    private Transform _destination;
    private Direction? _avoidanceDirection;
    private NpcAccessPoint.AccessPointType _dockedAPType;

    //private static readonly float HARD_AVOIDANCE_SPREAD = 6f;
    //private static readonly float HARD_AVOIDANCE_RANGE = 8f;

    private enum Direction
    {
        right,
        left,
        up,
        down
    }

    public GameObject Destination
    {
        get { return _destinationGO; }
    }

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        _npcType = GetComponent<NpcType>();
        avoidanceSpeed *= _rb.mass;
        _forwardSpeed = Random.Range(forwardSpeed.x, forwardSpeed.y) * _rb.mass;
	}

    void FixedUpdate()
    {
        if (!_isActive) return;
        if (_destination == null) ChooseNewDestination();

        _rb.AddForce(transform.forward * _forwardSpeed);
        PerformRotation();
    }

    public void Start(GameObject destination)
    {
        _destinationGO = destination;
        _destination = destination.transform;
        _isActive = true;
    }

    private void PerformRotation()
    {
        RaycastHit forwardHit, rightHit, leftHit, topHit, bottomHit; //, hardRightHit, hardLeftHit;
        bool forwardObstacle = Physics.Raycast(transform.position, transform.forward, out forwardHit, avoidanceRange);
        bool rightObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(avoidanceSpread, transform.up) * transform.forward, out rightHit, avoidanceRange);
        bool leftObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(-avoidanceSpread, transform.up) * transform.forward, out leftHit, avoidanceRange);
        bool topObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(avoidanceSpread, transform.right) * transform.forward, out topHit, avoidanceRange);
        bool bottomObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(-avoidanceSpread, transform.right) * transform.forward, out bottomHit, avoidanceRange);
        //bool hardRightObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(HARD_AVOIDANCE_SPREAD * avoidanceSpread, Vector3.up) * transform.forward, out hardRightHit, avoidanceRange);
        //bool hardLeftObstacle = Physics.Raycast(transform.position, Quaternion.AngleAxis(HARD_AVOIDANCE_SPREAD * -avoidanceSpread, Vector3.up) * transform.forward, out hardLeftHit, avoidanceRange);

        Debug.DrawLine(transform.position, transform.position + transform.forward * avoidanceRange);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(avoidanceSpread, transform.up) * transform.forward * avoidanceRange);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-avoidanceSpread, transform.up) * transform.forward * avoidanceRange);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(avoidanceSpread, transform.right) * transform.forward * avoidanceRange);
        Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-avoidanceSpread, transform.right) * transform.forward * avoidanceRange);
        //Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(HARD_AVOIDANCE_SPREAD * avoidanceSpread, Vector3.up) * transform.forward * (avoidanceRange / HARD_AVOIDANCE_RANGE));
        //Debug.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(HARD_AVOIDANCE_SPREAD * -avoidanceSpread, Vector3.up) * transform.forward * (avoidanceRange / HARD_AVOIDANCE_RANGE));

        if (_avoidanceDirection != null ||
            (forwardObstacle && !forwardHit.collider.CompareTag("NpcAccessPoint"))
            || (rightObstacle && !rightHit.collider.CompareTag("NpcAccessPoint"))
            || (leftObstacle && !leftHit.collider.CompareTag("NpcAccessPoint"))
            || (topObstacle && !topHit.collider.CompareTag("NpcAccessPoint"))
            || (bottomObstacle && !bottomHit.collider.CompareTag("NpcAccessPoint")))
        {
            if (rightObstacle || leftObstacle || topObstacle || bottomObstacle)
            {
                _avoidanceDirection = null;
                if ((rightObstacle && !rightHit.collider.CompareTag("NpcAccessPoint")) && !(leftObstacle && !leftHit.collider.CompareTag("NpcAccessPoint")))
                {
                    _rb.AddRelativeTorque(0f, -avoidanceSpeed, 0f);
                }
                if ((leftObstacle && !leftHit.collider.CompareTag("NpcAccessPoint")) && !(rightObstacle && !rightHit.collider.CompareTag("NpcAccessPoint")))
                {
                    _rb.AddRelativeTorque(0f, avoidanceSpeed, 0f);
                }
                if ((topObstacle && !topHit.collider.CompareTag("NpcAccessPoint")) && !(bottomObstacle && !bottomHit.collider.CompareTag("NpcAccessPoint")))
                {
                    _rb.AddRelativeTorque(-avoidanceSpeed, 0f, 0f);
                }
                if ((bottomObstacle && !bottomHit.collider.CompareTag("NpcAccessPoint")) && !(topObstacle && !topHit.collider.CompareTag("NpcAccessPoint")))
                {
                    _rb.AddRelativeTorque(avoidanceSpeed, 0f, 0f);
                }
            }
            else if ((forwardObstacle && !forwardHit.collider.CompareTag("NpcAccessPoint")) || _avoidanceDirection != null)
            {
                if (_avoidanceDirection == null)
                {
                    _avoidanceDirection = (Direction)Random.Range(0, 3);
                }
                switch (_avoidanceDirection)
                {
                    case Direction.right:
                        _rb.AddRelativeTorque(0f, avoidanceSpeed, 0f);
                        break;
                    case Direction.left:
                        _rb.AddRelativeTorque(0f, -avoidanceSpeed, 0f);
                        break;
                    case Direction.up:
                        _rb.AddRelativeTorque(avoidanceSpeed, 0f, 0f);
                        break;
                    case Direction.down:
                        _rb.AddRelativeTorque(-avoidanceSpeed, 0f, 0f);
                        break;
                }
            }
        }
        else
        {
            _avoidanceDirection = null;

            Vector3 targetDir = _destination.position - transform.position;
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotateSpeed * Time.deltaTime));
        }
    }

    private void ChooseNewDestination()
    {
        List<NpcAccessPoint> compatibleDestinations = NpcSpawnManager.instance.GetDestinationsForNpcType(_npcType, null);

        if (compatibleDestinations != null && compatibleDestinations.Count > 0)
        {
            GameObject destination = compatibleDestinations[Random.Range(0, compatibleDestinations.Count - 1)].gameObject;
            Start(destination);
            //_availableAccessPoints.Remove(accessPoint);
        }
    }

    public void Dock(NpcAccessPoint.AccessPointType type)
    {
        _dockedAPType = type;
        _isActive = false;
        var duration = 2f / rotateSpeed;
        var positionTween = transform.positionTo(duration, _destination.position);
        transform.rotationTo(duration, Quaternion.LookRotation(-_destination.forward).eulerAngles);
        positionTween.setOnCompleteHandler(InPositionToDock);
    }

    private void InPositionToDock(AbstractGoTween tween)
    {
        OnDestinationReached();

        NpcType npcType = GetComponent<NpcType>();
        if (npcType != null)
        {
            npcType.Dock(_dockedAPType);
        }
    }

    private void OnDestinationReached()
    {
        System.EventHandler handler = DestinationReached;
        if (handler != null)
        {
            handler(this, null);
        }
    }
}
