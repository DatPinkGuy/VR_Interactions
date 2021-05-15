using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapon
{
    [Serializable]
    public class Weapon : XRGrabInteractable
    {
        private bool switched;
        private GripInteract _gripInteract;
        private XRController _gripController;
        private HandguardInteract _handguardInteract;
        private XRBaseInteractor _handguardHand;
        private Barrel _barrel;
        private Rigidbody rb;
        private readonly Vector3 gripRot = new Vector3(0,0,0);
        private Quaternion lookRotation;
        //private ReloadArea _reloadArea;
        private Quaternion _originalRotation;
        private bool _foundOgRotation;
        [Tooltip("Value for two handed weapons to check distance between hands and drop weapon if it's too far")]
        [SerializeField] private float breakDistance = 0.25f;
        [SerializeField] private int recoilAmount = 25;
        [SerializeField] private bool oneHanded;
        [SerializeField] private bool isMachineGun;
        public bool Switched
        {
            get => switched;
            set => switched = value;
        }
        public XRBaseInteractor GripHand { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            SetUpHolds();
            SetUpExtra();
            selectEntered.AddListener(SetIniRotation);
            //_reloadArea = FindObjectOfType<ReloadArea>();
        }

        private void SetUpHolds()
        {
            _gripInteract = GetComponentInChildren<GripInteract>();
            _gripInteract.Setup(this);
            if (oneHanded) return; //Leave this for when i can change variables in inspector again
            _handguardInteract = GetComponentInChildren<HandguardInteract>();
            _handguardInteract.Setup(this);
        }

        private void SetUpExtra()
        {
            rb = GetComponent<Rigidbody>();
            _barrel = GetComponentInChildren<Barrel>();
            _barrel.Setup(this);
        }

        private void Update()
        {
            if (!_gripInteract) return;
            if(!isMachineGun)SwitchMode();
            else CalculateMachineGunPos();
        }

        private void SwitchMode()
        {
            if (_barrel.isOnlySingleFireMode) return;
            if (!_gripInteract.SwitchFireMode(_gripController)) return;
            if (switched) return;
            switched = true;
            _barrel.isOnSingleFireMode = !_barrel.isOnSingleFireMode;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            selectEntered.RemoveListener(SetIniRotation);
        }

        private void SetIniRotation(SelectEnterEventArgs args)
        {
            var newRotation = Quaternion.Euler(gripRot);
            args.interactor.attachTransform.localRotation = newRotation;
        }

        private void ResetToIniRotation(XRBaseInteractor interactor)
        {
            var newRotation = Quaternion.Euler(gripRot);
            interactor.attachTransform.localRotation = newRotation;
        }

        public void SetGripHand(SelectEnterEventArgs arg)
        {
            GripHand = arg.interactor;
            _gripController = GripHand.GetComponent<XRController>();
            OnSelectEntering(arg);
            OnSelectEntered(arg);
        }

        public void ClearGripHand(SelectExitEventArgs args)
        {
            GripHand = null;
            _gripController = null;
            try
            {
                _barrel.StopFire();
            }
            catch
            {
                // ignored
            }
            OnSelectExiting(args);
        }
    
        public void SetGuardHand(SelectEnterEventArgs args)
        {
            _handguardHand = args.interactor;
        }
    
        public void ClearGuardHand()
        {
            _handguardHand = null;
            ResetToIniRotation(GripHand);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (GripHand && _handguardHand && !isMachineGun) SetGripRotation(GetGripRotation());
            CheckDistance(GripHand, _gripInteract);
            CheckDistance(_handguardHand, _handguardInteract);
        }

        private Quaternion GetGripRotation()
        {
            var target = _handguardHand.attachTransform.position - GripHand.attachTransform.position;
            lookRotation = Quaternion.LookRotation(target);
            return lookRotation;
        }

        private void SetGripRotation(Quaternion _lookRotation)
        {
            var gripRotation = Vector3.zero;
            gripRotation.z = GripHand.transform.eulerAngles.z;
            _lookRotation *= Quaternion.Euler(gripRotation);
            GripHand.attachTransform.rotation = _lookRotation;
            //_handguardHand.transform.rotation = _gripHand.transform.rotation;
        }
    
        private void CheckDistance(XRBaseInteractor interactor, HandHold handHold)
        {
            if (!interactor) return;
            var disSqr = GetDistanceSqrToInteractor(interactor);
            if (disSqr > breakDistance) handHold.BreakHold(new SelectExitEventArgs());
        }

        private void CalculateMachineGunPos()
        {
            if (!GripHand) return;
            if (!_handguardHand)
            {
                GetAngle(GripHand.transform.position);
                return;
            }
            var vector = _handguardHand.transform.position +
                         (GripHand.transform.position - _handguardHand.transform.position) / 2;
            GetAngle(vector);
        }

        public void PullTrigger()
        {
            _barrel.Fire();
        }

        public void ReleaseTrigger()
        {
            _barrel.StopFire();
            _barrel.SingleFired = false;
        }

        // public void Recoil() //Needs Implementing proper interactions
        // {
        //     //rb.AddRelativeForce(Vector3.back*recoilAmount, ForceMode.Impulse); not nice
        // }

        public void UseVibration()
        {
            if (!GripHand) return;
            _gripInteract.Vibrate();
        }

        public bool CheckLaser()
        {
            if (!GripHand) return false;
            if (_gripInteract.TurnOnOffLaser(_gripController)) return true;
            _barrel.LaserSwitched = false;
            return false;
        }
        
        
        //MachineGun
        private void GetAngle(Vector3 sentVector)
        {
            if (!GripHand) return;
            var vector = transform.position - sentVector;
            transform.rotation = Quaternion.LookRotation(vector);
        }


        //Previously used for having a Reload Area
        // private void OnTriggerEnter(Collider other)
        // {
        //     //if (!_gripHand) return;
        //     //if (other != _reloadArea.Collider) return;
        //     //if(!isMachineGun) _barrel.Reload();
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     //if (!_gripHand) return;
        //     //if (other != _reloadArea.Collider) return;
        //     //if(!isMachineGun) _barrel.StopReload();
        // }
    }
}
