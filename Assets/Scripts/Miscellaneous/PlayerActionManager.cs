using System.Collections;
using UnityEngine;
using Weaponry;

namespace Miscellaneous
{
    public class PlayerActionManager : MonoBehaviour
    {
        
        [SerializeField] private Transform _weaponPosition;
        [SerializeField] private Transform _weaponDropPosition;

        private GameObject _currentWeapon;
        private IWeapon _currentWeaponScript;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Weapon"))
            {
                PickUpWeapon(other.gameObject);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && _currentWeapon != null)
            {
                DropWeapon(_weaponDropPosition, _currentWeapon);
            }

            if(Input.GetMouseButton(0))
            {
                if (_currentWeapon != null && _currentWeaponScript != null)
                {
                    _currentWeaponScript.Shoot();
                }
            }
        }

        private void PickUpWeapon(GameObject weapon)
        {
            // Сбрасываем оружие, которое уже имеем
            if(_currentWeapon != null)
                DropWeapon(_weaponDropPosition, _currentWeapon);

            _currentWeapon = weapon;
            _currentWeaponScript = weapon.GetComponent<IWeapon>();

            // Отключаем анимацию вращения оружия
            WeaponIdleAnimation animationScript = weapon.gameObject.GetComponent<WeaponIdleAnimation>();
            if (animationScript) animationScript.enabled = false;

            // Отключаем коллайдер, чтобы не срабатывал триггер повторно
            Collider weaponCollider = weapon.GetComponent<Collider>();
            if (weaponCollider != null)
                weaponCollider.enabled = false;

            // Делаем оружие дочерним объектом позиции оружия
            weapon.transform.SetParent(_weaponPosition);

            // Совмещаем трансформы
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;

            // Можно (по желанию) сбросить масштаб, если он должен совпадать
            weapon.transform.localScale = Vector3.one;
        }

        private void DropWeapon(Transform dropPosition, GameObject weapon)
        {
            // Отсоединяем оружие от родителя (например, от рук игрока)
            weapon.transform.SetParent(null);

            dropPosition.rotation = Quaternion.identity;
            // Ставим оружие в точку сброса
            weapon.transform.position = dropPosition.position;
            weapon.transform.rotation = dropPosition.rotation;

            // Включаем анимацию вращения оружия
            WeaponIdleAnimation animationScript = weapon.GetComponent<WeaponIdleAnimation>();
            if (animationScript) animationScript.enabled = true;

            // Выключаем коллайдер, чтобы игрок не подобрал оружие мгновенно
            Collider weaponCollider = weapon.GetComponent<Collider>();
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
                // Запускаем корутину для повторного включения через 1.5 секунды
                StartCoroutine(EnableColliderLater(weaponCollider, 1.5f));
            }
            _currentWeapon = null;
            _currentWeaponScript = null;
        }

        private IEnumerator EnableColliderLater(Collider collider, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (collider != null)
                collider.enabled = true;
        }
    }
}
