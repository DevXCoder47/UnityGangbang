using Miscellaneous;
using System.Collections;
using UnityEngine;
using Weaponry;

namespace NPC
{
    public class NPCActionManager : MonoBehaviour
    {
        [SerializeField] private Transform _weaponPosition;
        [SerializeField] private Transform _weaponDropPosition;
        [SerializeField] private float _scanRadius;
        [SerializeField] private string _scanningTag;

        private GameObject _currentWeapon;
        private IWeapon _currentWeaponScript;
        private NPCMovingController _controllerScript;
        private Rotation _rotationScript;
        private Transform _nearestEnemy;
        private bool isAtacking;

        public NPCMovingController ControllerScript { get; }
        public Rotation RotationScript { get; }

        void Start()
        {
            _controllerScript = GetComponent<NPCMovingController>();
            _rotationScript = GetComponent<Rotation>();
            ScanForWeapons();
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Weapon"))
            {
                PickUpWeapon(other.gameObject);
            }
        }

        void Update()
        {
            
            // Если цель достигнута и NPC стоит без оружия — продолжаем искать
            if (_controllerScript.Target == null && _currentWeapon == null)
            {
                ScanForWeapons();
            }
            if (_currentWeapon != null && _nearestEnemy == null)
            {
                ScanForEnemies();
            }
            if (_nearestEnemy != null)
            {
                StartCoroutine(StartAttackingLater());
            }
            if (isAtacking)
            {
                _currentWeaponScript.Shoot();
                if (_controllerScript.Agent.updateRotation) _controllerScript.Agent.updateRotation = false;
                if (_rotationScript.Target == null && _nearestEnemy != null) _rotationScript.Target = _nearestEnemy.transform;
            }
            if (_nearestEnemy == null && isAtacking)
            {
                isAtacking = false;
            }
        }

        void OnDestroy()
        {
            if(_currentWeapon != null)
            {
                DropWeaponAfterDeath(_weaponDropPosition, _currentWeapon);
            }
        }

        private void ScanForWeapons()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);
            float nearestDistance = float.MaxValue;
            Transform nearestWeapon = null;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Weapon"))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestWeapon = hit.transform;
                    }
                }
            }

            if (nearestWeapon != null)
            {
                _controllerScript.Target = nearestWeapon;
            }
        }

        private void ScanForEnemies()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);
            float nearestDistance = float.MaxValue;
            Transform nearestEnemy = null;

            foreach (var hit in hits)
            {
                if (hit.CompareTag(_scanningTag) || (gameObject.CompareTag("Enemy") && hit.CompareTag("Player")))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = hit.transform;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                _nearestEnemy = nearestEnemy;
                _controllerScript.Target = _nearestEnemy;
                _controllerScript.Agent.stoppingDistance = 25f;
            }
        }

        private void PickUpWeapon(GameObject weapon)
        {
            // Сбрасываем оружие, которое уже имеем
            if (_currentWeapon != null)
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

        private void DropWeaponAfterDeath(Transform dropPosition, GameObject weapon)
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
                weaponCollider.enabled = true;
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

        private IEnumerator StartAttackingLater()
        {
            yield return new WaitForSeconds(2);
            isAtacking = true;
        }
    }
}
