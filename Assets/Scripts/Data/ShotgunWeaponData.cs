using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ShotgunWeaponData", menuName = "Game Data/Weapon/Shotgun")]
    public class ShotgunWeaponData : WeaponData
    {
        [Header("Shotgun settings")]
        public int pelletsCount;
        public float spreadAngle;
    }
}
