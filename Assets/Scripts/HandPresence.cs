using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    private InputDevice _currentController;
    private GameObject _spawnedHand;
    private Animator HandAnimator => _spawnedHand.GetComponent<Animator>();
    [SerializeField] private InputDeviceCharacteristics controllerCharacteristics;
    [SerializeField] private GameObject controllerPrefab;
    private static readonly int Trigger = Animator.StringToHash("Trigger");
    private static readonly int Grip = Animator.StringToHash("Grip");

    // Start is called before the first frame update
    void Start()
    {
       InitializeController();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_currentController.isValid) InitializeController();
        UpdateAnimation();
    }

    private void InitializeController()
    {
        var controller = new List<InputDevice>(1);
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics,controller);
        if (controller.Count == 0) return;
        _currentController = controller[0];
        _spawnedHand = Instantiate(controllerPrefab, transform);
    }
    
    private void UpdateAnimation()
    {
        if (!HandAnimator) return;
        if (_currentController.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue))
        {
            HandAnimator.SetFloat(Trigger, triggerValue);
        }
        else HandAnimator.SetFloat(Trigger, 0);
        
        if (_currentController.TryGetFeatureValue(CommonUsages.grip, out var gripValue))
        {
            HandAnimator.SetFloat(Grip, gripValue);
        }
        else HandAnimator.SetFloat(Grip, 0);
    }
}
