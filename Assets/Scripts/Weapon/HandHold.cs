using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    [Serializable] 
    public class HandHold : XRBaseInteractable
    {
        [SerializeField] private bool handsVisible; //Unity stopped showing variable in editor
        protected Weapon Weapon;
        protected XRController Controller;

        public void Setup(Weapon weapon)
        {
            Weapon = weapon;
        }
        protected override void Awake()
        {
            base.Awake();
            activated.AddListener(BeginAction);
            deactivated.AddListener(EndAction);
            selectEntered.AddListener(Grab);
            selectExited.AddListener(Drop);
        }

        private void OnDestroy()
        {
            Controller = null;
            activated.RemoveListener(BeginAction);
            deactivated.RemoveListener(EndAction);
            selectEntered.RemoveListener(Grab);
            selectExited.RemoveListener(Drop);
        }

        protected virtual void BeginAction(ActivateEventArgs args)
        {
        
        }
    
        protected virtual void EndAction(DeactivateEventArgs args)
        {
        
        }
    
        public void BreakHold(SelectExitEventArgs args)
        {
            Drop(args);
        }

        protected virtual void Grab(SelectEnterEventArgs args)
        {
            if(!handsVisible) TryHideHand(args.interactor, false);
        }

        protected virtual void Drop(SelectExitEventArgs args)
        {
            if(!handsVisible) TryHideHand(args.interactor, true);
        }

        public void TryHideHand(XRBaseInteractor interactor, bool hide)
        {
            if (interactor is XRDirectInteractor hand)
                interactor.GetComponentInChildren<SkinnedMeshRenderer>().enabled = hide;
        }
    }
}
