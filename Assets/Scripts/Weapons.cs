﻿namespace VoidWars { 
    /// <summary>
    /// Factory class for weapons.
    /// </summary>
    public static class Weapons {
        /// <summary>
        /// Factory method for making a weapon.
        /// </summary>
        /// <param name="weaponClass">The immutable weapon data.</param>
        /// <returns>An appropriate weapon instance.</returns>
        public static WeaponInstance CreateWeapon(WeaponClass weaponClass) {
            switch(weaponClass.WeaponType) {
                case WeaponType.Laser:
                case WeaponType.UVLaser:
                    return new Laser(weaponClass);

                case WeaponType.RailGun:
                    return new RailGun(weaponClass);

                case WeaponType.HomingMissile:
                    return new HomingMissileLauncher(weaponClass);

                default:
                    return null;
            }
        }
    }
}