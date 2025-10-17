using UnityEngine;

namespace Miscellaneous
{
    public class WeaponIdleAnimation : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;

        void Update()
        {
            // Вращаем оружие
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
