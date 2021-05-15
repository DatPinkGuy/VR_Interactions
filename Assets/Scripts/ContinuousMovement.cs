using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{
    private InputDevice Device => InputDevices.GetDeviceAtXRNode(inputSource);
    private InputDevice RotDevice => InputDevices.GetDeviceAtXRNode(rotationSource);
    private Vector2 _inputAxis;
    private Vector2 _inputRotation;
    private bool _inputAxisClick;
    private Vector3 _direction;
    private Quaternion _headYaw;
    private XRRig _xrRig;
    private float _fallingSpeed;
    private bool _isGrounded;
    private bool _hasHit;
    private Vector3 _rayStart;
    private float _rayLength;
    private Vector3 _capsuleCenter;
    private readonly Collider[] _collisionCheck = new Collider[1];
    private float rotateValue;
    private bool usingSnapTurn;
    private DeviceBasedSnapTurnProvider SnapRotation => GetComponent<DeviceBasedSnapTurnProvider>();
    [SerializeField] private CharacterController character;
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float additionalHeight;
    [SerializeField] private bool movementLocked;
    [SerializeField] private XRNode inputSource;
    [SerializeField] private XRNode rotationSource;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask bodyLayer;
    [Range(1f,3f)] [SerializeField] private float rotationDamp;
    // Start is called before the first frame update
    void Start()
    {
        _xrRig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        Device.TryGetFeatureValue(CommonUsages.primary2DAxis, out _inputAxis);
        Device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out _inputAxisClick);
        RotDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out _inputRotation); //use X value for rotation
    }

    private void FixedUpdate()
    {
        CheckIfFar();
        HeadMovement();
    }

    private void HeadMovement()
    {
        if (!movementLocked)
        {
            //Move character based on controller input
            _headYaw = Quaternion.Euler(0,_xrRig.cameraGameObject.transform.eulerAngles.y,0);
            _direction = _headYaw * new Vector3(_inputAxis.x, 0, _inputAxis.y);
            if (!_inputAxisClick) character.Move(_direction * (Time.deltaTime * speed));
            else character.Move(_direction * (Time.deltaTime * (speed * 2)));
            //Moves Character Controller to camera when player moves
            if (_direction != Vector3.zero) CharacterControllerFollow();
            if(!usingSnapTurn) SmoothRotate();
            //Checks if gravity needs to be applied
            //Going downhill is jittery
        }
        _isGrounded = CheckIfGrounded();
        if (_isGrounded) _fallingSpeed = 0;
        else _fallingSpeed += gravity * Time.fixedDeltaTime;
        character.Move(Vector3.up * (_fallingSpeed * Time.fixedDeltaTime));
    }

    private void SmoothRotate()
    {
        if (!(Math.Abs(_inputRotation.x) > 0)) return;
        rotateValue = _inputRotation.x / rotationDamp;
        _xrRig.RotateAroundCameraUsingRigUp(rotateValue);
    }

    private bool CheckIfGrounded()
    {
        _rayStart = transform.TransformPoint(character.center);
        _rayLength = character.center.y + 0.01f;
        _hasHit = Physics.SphereCast(_rayStart, character.radius, Vector3.down, out RaycastHit hit, _rayLength, groundLayer);
        Debug.DrawRay(_rayStart,Vector3.down * _rayLength,Color.white );
        return _hasHit;
    }

    private void CheckIfFar()
    {
        character.height = _xrRig.cameraInRigSpaceHeight + additionalHeight;
        if (Physics.OverlapSphereNonAlloc(_xrRig.cameraGameObject.transform.position, 0.5f, _collisionCheck,
            bodyLayer) > 0)
        {
            CharacterControllerHeight();
            return;
        }
        CharacterControllerHeight();
        CharacterControllerFollow();
    }

    private void CharacterControllerFollow()
    {
        _capsuleCenter = transform.InverseTransformPoint(_xrRig.cameraGameObject.transform.position);
    }

    private void CharacterControllerHeight()
    {
        character.height = _xrRig.cameraInRigSpaceHeight + additionalHeight;
        character.center = new Vector3(_capsuleCenter.x, character.height/2 + character.skinWidth, _capsuleCenter.z);
    }

    public void SwitchTurning() //apply to be used in settings later (has been checked, works)
    {
        if (!usingSnapTurn)
        {
            usingSnapTurn = true;
            SnapRotation.enabled = true;
        }
        else
        {
            usingSnapTurn = false;
            SnapRotation.enabled = false;
        }
    }
}
