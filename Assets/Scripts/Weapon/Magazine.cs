using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    public class Magazine : XRGrabInteractable
    {
        public enum MagType
        {
            M4A1,
            Cal50,
            MM9
        }
        public MagType magType;
        private Rigidbody rb => GetComponent<Rigidbody>();
        private bool alreadyRan;
        [Header("Magazine Parameters")]
        [SerializeField] private bool isInUse;
        [SerializeField] private bool isInHand;
        [SerializeField] private int magSize;
        [SerializeField] private int currentSize;
        [SerializeField] private GameObject objectToDisable;
        public int CurrentSize
        {
            get => currentSize;
            set => currentSize = value;
        }
        public Transform MagSlotT { get; set; }
        public GameObject ObjectToDisable { get; private set; }
        public bool IsInUse
        {
            set => isInUse = value;
        }
        public bool IsInHand => isInHand;

        protected override void Awake()
        {
            base.Awake();
            selectEntered.AddListener(Grab);
            selectExited.AddListener(Drop);
            selectExited.AddListener(ResetParentMethod);
            currentSize = magSize;
        }

        public void RemoveHand(SelectExitEventArgs args)
        {
            OnSelectExiting(args);
            Drop(args);
            alreadyRan = true;
        }

        private void Grab(SelectEnterEventArgs args)
        {
            isInHand = true;
            alreadyRan = false;
        }
        
        private void Drop(SelectExitEventArgs args)
        {
            SwitchState();
        }

        public void SwitchState()
        {
            if (!isInUse)
            {
                transform.parent = null;
                rb.isKinematic = false;
                rb.useGravity = true;
                isInHand = false;
            }
            else
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                isInHand = false;
            }
        }

        private void ResetParentMethod(SelectExitEventArgs args)
        {
            if (!alreadyRan) return;
            if (!isInUse) return;
            StartCoroutine(ResetParent());
        }
        
        private IEnumerator ResetParent()
        {
            yield return new WaitForEndOfFrame();
            transform.parent = MagSlotT.parent;
        }

        public void DisableChildObject()
        {
            if (!objectToDisable) return;
            if (currentSize > 0) return;
            objectToDisable.SetActive(false);
        }
    }
}
