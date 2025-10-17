using UnityEngine;

namespace Miscellaneous
{
    public class WeaponPicker : MonoBehaviour
    {
        [SerializeField] private Transform _weaponPos;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Weapon"))
            {
                PickUpWeapon(other.gameObject);
            }
        }
        private void PickUpWeapon(GameObject weapon)
        {
            // Отключаем анимацию вращения оружия
            WeaponIdleAnimation animationScript = weapon.gameObject.GetComponent<WeaponIdleAnimation>();
            if(animationScript) animationScript.enabled = false;

            // Отключаем коллайдер, чтобы не срабатывал триггер повторно
            Collider weaponCollider = weapon.GetComponent<Collider>();
            if (weaponCollider != null)
                weaponCollider.enabled = false;

            // Делаем оружие дочерним объектом позиции оружия
            weapon.transform.SetParent(_weaponPos);

            // Совмещаем трансформы
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;

            // Можно (по желанию) сбросить масштаб, если он должен совпадать
            weapon.transform.localScale = Vector3.one;
        }
    }
}
