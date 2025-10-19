using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game Data/Weapon")]
    public abstract class WeaponData : ScriptableObject
    {
        [Header("Weapon stats")]
        public float fireRate;
        public float damage;

        [Header("Raycast settings")]
        public float shootDistance; 
        public LayerMask hitLayers;

        [Header("Tracer settings")]
        public LineRenderer tracerPrefab;   
        public float tracerDuration = 1; 

        [Header("Recoil settings")]
        public float baseSpread;   
        public float maxSpread;      
        public float spreadIncrease; 
        public float spreadDecrease;

        [Header("Sound settings")]
        public AudioClip shotSound;       

        [Header("Muzzle flash settings")]
        public GameObject muzzleFlashPrefab;
        public float muzzleFlashDuration;
    }
}
