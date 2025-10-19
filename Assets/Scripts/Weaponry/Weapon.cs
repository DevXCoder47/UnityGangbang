using Data;
using Miscellaneous;
using UnityEngine;
namespace Weaponry
{
    public class Weapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected WeaponData data;

        [SerializeField] protected Transform muzzlePosition;

        protected float lastShotTime; // время последнего выстрела
        protected float currentSpread; // текущий разброс


        protected virtual void Start()
        {
            // Начинаем с базового разброса
            if (data is RangedWeaponData rangedData)
                currentSpread = rangedData.baseSpread;
        }

        protected virtual void Update()
        {
            // Постепенно уменьшаем разброс со временем, если не стреляем
            if (data is RangedWeaponData rangedData)
            {
                currentSpread = Mathf.MoveTowards(
                    currentSpread,
                    rangedData.baseSpread,
                    rangedData.spreadDecrease * Time.deltaTime
                );
            }
        }
        public virtual void Shoot() 
        {
            if (!(data is RangedWeaponData rangedData))
                return;

            // 1. Ограничение скорострельности
            if (Time.time - lastShotTime < rangedData.fireRate)
                return;

            lastShotTime = Time.time; // обновляем момент последнего выстрела

            // 2. — Расчёт направления выстрела с разбросом
            Vector3 shootDir = GetShootDirection(rangedData);

            // 3. — Выполняем Raycast
            Vector3 start = muzzlePosition.position;
            Vector3 end = start + shootDir * rangedData.shootDistance;

            if (Physics.Raycast(start, shootDir, out RaycastHit hit, rangedData.shootDistance, rangedData.hitLayers))
            {
                end = hit.point;

                // Если у цели есть Health — наносим урон
                var health = hit.collider.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(rangedData.damage);
            }

            // 4. — Эффекты
            SpawnTracer(start, end);
            SpawnMuzzleFlash();
            PlayShotSound();

            // Увеличиваем разброс после выстрела
            currentSpread = Mathf.Min(currentSpread + rangedData.spreadIncrease, rangedData.maxSpread);
        }
        protected virtual void SpawnTracer(Vector3 start, Vector3 end)
        {
            if (!(data is RangedWeaponData rangedData) || rangedData.tracerPrefab == null)
                return;

            LineRenderer tracer = Instantiate(rangedData.tracerPrefab, start, Quaternion.identity);
            tracer.SetPosition(0, start);
            tracer.SetPosition(1, end);

            // Уничтожаем трассер через заданное время
            Destroy(tracer.gameObject, rangedData.tracerDuration);
        }
        protected virtual void SpawnMuzzleFlash()
        {
            if (!(data is RangedWeaponData rangedData) || rangedData.muzzleFlashPrefab == null)
                return;

            GameObject flash = Instantiate(
                rangedData.muzzleFlashPrefab,
                muzzlePosition.position,
                muzzlePosition.rotation,
                muzzlePosition
            );

            Destroy(flash, rangedData.muzzleFlashDuration);
        }
        protected virtual void PlayShotSound()
        {
            if (!(data is RangedWeaponData rangedData) || rangedData.shotSound == null)
                return;

            AudioSource.PlayClipAtPoint(rangedData.shotSound, transform.position);
        }

        protected Vector3 GetShootDirection(RangedWeaponData rangedData)
        {
            Vector3 direction = muzzlePosition.forward;

            // Добавляем текущий разброс
            float spreadX = Random.Range(-currentSpread, currentSpread);
            float spreadY = Random.Range(-currentSpread, currentSpread);
            Vector3 spreadVector = muzzlePosition.TransformDirection(new Vector3(spreadX, spreadY, 0));

            return (direction + spreadVector).normalized;
        }
    }
}
