using System;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        private List<Weapon> _weaponList;

        private void OnValidate()
        {
            _weaponList = new List<Weapon>();
            _weaponList.AddRange(FindObjectsOfType<Weapon>());
            foreach (var weapon in _weaponList)
            {
                weapon.enabled = false;
            }
        }

        void Start()
        {
            foreach (var weapon in _weaponList)
            {
                weapon.enabled = true;
            }
        }
    }
}
