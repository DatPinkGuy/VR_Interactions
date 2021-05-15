using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    public class GripInteract : HandHold
    {
        private bool _isPressed;
        private bool _secondaryButton;
        private bool _primaryButton;
        protected override void BeginAction(ActivateEventArgs args)
        {
            base.BeginAction(args);
            Controller = args.interactor.GetComponent<XRController>();
            Controller.inputDevice.IsPressed(InputHelpers.Button.Trigger, out _isPressed);
            //If i want the button press to be registered when it's almost pressed in, have to continuously check
            //for IsPressed instead of (how it seems) being checked once on BeginAction. Default threshold 0.1f
            if(_isPressed) Weapon.PullTrigger();
        }

        protected override void EndAction(DeactivateEventArgs args)
        {
            base.EndAction(args);
            Controller = args.interactor.GetComponent<XRController>();
            Controller.inputDevice.IsPressed(InputHelpers.Button.Trigger, out _isPressed);
            if (_isPressed) return;
            Weapon.ReleaseTrigger();
        }

        protected override void Grab(SelectEnterEventArgs args)
        {
            base.Grab(args);
            Weapon.SetGripHand(args);
        }

        protected override void Drop(SelectExitEventArgs args)
        {
            base.Drop(args);
            Weapon.ClearGripHand(args);
        }

        public void Vibrate()
        {
            Controller.inputDevice.SendHapticImpulse(0, 1f);
        }

        public bool SwitchFireMode(XRController controller)
        {
            if (!controller) return false;
            controller.inputDevice.IsPressed(InputHelpers.Button.SecondaryButton, out _secondaryButton);
            if (_secondaryButton == false) Weapon.Switched = false;
            return _secondaryButton;
        }
        
        public bool TurnOnOffLaser(XRController controller)
        {
            if (!controller) return false;
            controller.inputDevice.IsPressed(InputHelpers.Button.PrimaryButton, out _primaryButton);
            if (_primaryButton == false)
            {
            }
            return _primaryButton;
        }
    }
}
