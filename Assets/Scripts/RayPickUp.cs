using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayPickUp : MonoBehaviour
{
    private RaycastHit _rayHit;
    private GameObject _hoveredItem;
    private GameObject _lastKnownHovered;
    private bool _isCoroutineRunning;
    private XRController HandXRInteractor => GetComponent<XRController>();
    private bool _thresholdBroken;
    private Rigidbody HoveredItemRb => _lastKnownHovered.GetComponent<Rigidbody>();
    [Header("Flick values")]
    [SerializeField] private float beginThreshold;
    [SerializeField] private float endThreshold;
    [SerializeField] private float launchAngle;
    [Header("Line Renderer values")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject handForPickUp;
    [SerializeField] private LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        LineRendererSwitch();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForItem();
    }

    private void CheckForItem()
    {
        _hoveredItem = Physics.BoxCast(handForPickUp.transform.position, new Vector3(0.1f, 0.1f, 0.1f),
            handForPickUp.transform.forward, out _rayHit,
            Quaternion.identity, 10f, layerMask) ? _rayHit.transform.gameObject : null;
        if (!_hoveredItem) return;
        if (_isCoroutineRunning) return;
        StartCoroutine(DrawRay());
    }

    private IEnumerator DrawRay()
    {
        _lastKnownHovered = _hoveredItem;
        _isCoroutineRunning = true;
        LineRendererSwitch();
        while (TriggerButtonCheck() || _hoveredItem)
        {
            if(TriggerButtonCheck())
            {
                CheckForFlick();
                lineRenderer.SetPositions(new[]{handForPickUp.transform.position, _lastKnownHovered.transform.position});
                yield return null;
            }
            else
            {
                if (_hoveredItem != _lastKnownHovered) _lastKnownHovered = _hoveredItem;
                lineRenderer.SetPositions(new[]{handForPickUp.transform.position, _hoveredItem.transform.position});
                yield return null;
            }
        }
        LineRendererSwitch();
        _isCoroutineRunning = false;
    }

    private void LineRendererSwitch()
    {
        lineRenderer.enabled = !lineRenderer.enabled;
    }

    private void CheckForFlick()
    {
        var rotSpeed = GetRotSpeed();
        var speed = GetSpeed();
        if (!_thresholdBroken)
        {
            _thresholdBroken = rotSpeed > beginThreshold && speed > beginThreshold;
        }
        if (!_thresholdBroken) return;
        if (!(rotSpeed < endThreshold)) return;
        ThrowObject();
        _thresholdBroken = false;
    }

    private Vector3 GetValue(InputFeatureUsage<Vector3> usage)
    {
        HandXRInteractor.inputDevice.TryGetFeatureValue(usage, out var value);
        return value;
    }

    private float GetSpeed()
    {
        return GetValue(CommonUsages.deviceVelocity).magnitude;
    }

    private float GetRotSpeed()
    {
        return GetValue(CommonUsages.deviceAngularVelocity).magnitude;
    }

    private void ThrowObject()
    {
        //if (!TriggerButtonCheck()) return;
        LaunchObject();
    }

    private bool TriggerButtonCheck()
    {
        HandXRInteractor.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out var triggerCheck);
        return triggerCheck;
    }


    private void LaunchObject()
    {
        var handPosition = handForPickUp.transform.position;
        var gravity = Physics.gravity.magnitude;
        var angle = launchAngle * Mathf.Deg2Rad;
        var planarTarget = new Vector3(handPosition.x, 0, handPosition.z);
        var planarPosition = new Vector3(_lastKnownHovered.transform.position.x, 0, _lastKnownHovered.transform.position.z);
        var distance = Vector3.Distance(planarTarget, planarPosition);
        var yOffset = _lastKnownHovered.transform.position.y - handPosition.y;
        var initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        var velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
        var angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPosition) * (handPosition.x > _lastKnownHovered.transform.position.x ? 1 : -1);
        var finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
        HoveredItemRb.velocity = finalVelocity;
    }
}
