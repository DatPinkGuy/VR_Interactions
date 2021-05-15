using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Toolbelt : MonoBehaviour
{
    private XRRig _xrRig;
    [SerializeField] private float height;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform rigTarget;
    // Start is called before the first frame update
    void Start()
    {
        _xrRig = rigTarget.GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBelt();
    }

    private void MoveBelt()
    {
        var adjustedHeight = cameraTarget.position;
        adjustedHeight.y = Mathf.Lerp(adjustedHeight.y - _xrRig.cameraInRigSpaceHeight, adjustedHeight.y, height);
        transform.localPosition = adjustedHeight;

        var adjustedRotation = cameraTarget.localEulerAngles + rigTarget.localEulerAngles;
        adjustedRotation.x = 0;
        adjustedRotation.z = 0;
        transform.localEulerAngles = adjustedRotation;
    }
}
