using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    public class MagazineSlot : MonoBehaviour
    {
        public enum MagType
        {
            M4A1,
            Cal50,
            MM9
        }
        public MagType magType;
        [SerializeField] private bool isMagazineAttached;
        [SerializeField] private Magazine currentMagazine;
        [SerializeField] private AudioClip magIn;
        [SerializeField] private AudioClip magOut;
        [SerializeField] private Transform magPivot;
        private AudioSource audioSource;
        public Magazine CurrentMagazine => currentMagazine;
        private Transform currentMagT => currentMagazine.GetComponent<Transform>();

        private Transform thisT => transform;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnTriggerEnter(Collider other) //Can pick up from ground, but messes up
        {
            if (isMagazineAttached) return;
            var otherObj = other.GetComponent<Magazine>();
            if (!otherObj) return;
            if (currentMagazine == otherObj) return;
            if ((int) magType != (int) otherObj.magType) return;
            other.isTrigger = true; //TESTING
            currentMagazine = otherObj;
            isMagazineAttached = true;
            currentMagazine.IsInUse = true;
            //if (magIn != null && currentMagazine.IsInHand) audioSource.PlayOneShot(magIn);
            if (currentMagazine.IsInHand) RemoveHand(new SelectExitEventArgs());
            else currentMagazine.SwitchState();
            audioSource.PlayOneShot(magIn);
            currentMagazine.MagSlotT = thisT;
            currentMagT.parent = thisT.parent;
            currentMagT.position = magPivot.position;
            currentMagT.rotation = magPivot.rotation;
            Debug.Log("TriggerEnter");
        }

        private void OnTriggerExit(Collider other) //Needs improvement?
        {
            var otherObj = other.GetComponent<Magazine>();
            if (!otherObj) return;
            if ((int) magType != (int) otherObj.magType) return;
            if (!currentMagazine.IsInHand) return;
            Debug.Log("TriggerExit");
            other.isTrigger = false; //TESTING
            currentMagazine.IsInUse = false;
            isMagazineAttached = false;
            if (magOut && currentMagazine.IsInHand) audioSource.PlayOneShot(magOut);
            currentMagazine = null;
        }

        private void RemoveHand(SelectExitEventArgs args)
        {
            currentMagazine.RemoveHand(args);
        }
    }
}