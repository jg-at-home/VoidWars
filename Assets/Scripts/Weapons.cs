namespace VoidWars { 
    /// <summary>
    /// Factory class for weapons.
    /// </summary>
    public static class Weapons {
        public static WeaponInstance CreateWeapon(WeaponClass weaponClass) {
            switch(weaponClass.WeaponType) {
                case WeaponType.Laser:
                case WeaponType.UVLaser:
                    return new Laser(weaponClass);

                case WeaponType.EMP:
                    return new EmpWeapon(weaponClass);

                case WeaponType.RailGun:
                    return new RailGun(weaponClass);

                default:
                    return null;
            }
        }
    }
}