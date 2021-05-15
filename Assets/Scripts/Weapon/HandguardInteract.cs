using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    public class HandguardInteract : HandHold
    {
        protected override void BeginAction(ActivateEventArgs args)
        {
            base.BeginAction(args);
        }

        protected override void EndAction(DeactivateEventArgs args)
        {
            base.EndAction(args);
        }

        protected override void Grab(SelectEnterEventArgs args)
        {
            
            base.Grab(args);
            Weapon.SetGuardHand(args);
        }

        protected override void Drop(SelectExitEventArgs args)
        {
            base.Drop(args);
            Weapon.ClearGuardHand();
        }
    }
}
