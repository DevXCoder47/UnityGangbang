using Data;
using Miscellaneous;
using UnityEngine;

namespace Weaponry
{
    public class Shotgun : Weapon
    {
        public override void Shoot()
        {
            if (!(data is ShotgunWeaponData shotgunData))
                return;

            // Ограничение скорострельности
            if (Time.time - lastShotTime < shotgunData.fireRate)
                return;

            lastShotTime = Time.time;

            // Точка старта выстрела
            Vector3 start = muzzlePosition.position;

            // 🔸 Для каждой дробинки — Raycast
            for (int i = 0; i < shotgunData.pelletsCount; i++)
            {
                // Генерируем направление с разбросом
                Vector3 shootDir = GetPelletDirection(shotgunData.spreadAngle);

                Vector3 end = start + shootDir * shotgunData.shootDistance;

                if (Physics.Raycast(start, shootDir, out RaycastHit hit, shotgunData.shootDistance, shotgunData.hitLayers))
                {
                    end = hit.point;

                    // Наносим урон
                    if (hit.collider.TryGetComponent<Health>(out var health))
                        health.TakeDamage(shotgunData.damage);
                }

                // Отрисовываем трассер
                SpawnTracer(start, end);
            }

            // Вспышка и звук
            SpawnMuzzleFlash();
            PlayShotSound();
        }

        /// <summary>
        /// Возвращает случайное направление внутри cone (spreadAngle)
        /// </summary>
        private Vector3 GetPelletDirection(float spreadAngle)
        {
            // направление вперёд от дула
            Vector3 forward = muzzlePosition.forward;

            // случайный угол отклонения
            float angle = Random.Range(0f, spreadAngle);
            float azimuth = Random.Range(0f, 360f);

            // формируем вектор
            Quaternion rotation = Quaternion.Euler(angle * Mathf.Sin(azimuth), angle * Mathf.Cos(azimuth), 0);
            Vector3 dir = rotation * forward;

            return dir.normalized;
        }

        protected override void SpawnTracer(Vector3 start, Vector3 end)
        {
            if (!(data is ShotgunWeaponData rangedData) || rangedData.tracerPrefab == null)
                return;

            LineRenderer tracer = Instantiate(rangedData.tracerPrefab, start, Quaternion.identity);
            tracer.SetPosition(0, start);
            tracer.SetPosition(1, end);

            // Уничтожаем трассер через заданное время
            Destroy(tracer.gameObject, rangedData.tracerDuration);
        }

        protected override void SpawnMuzzleFlash()
        {
            if (!(data is ShotgunWeaponData rangedData) || rangedData.muzzleFlashPrefab == null)
                return;

            GameObject flash = Instantiate(
                rangedData.muzzleFlashPrefab,
                muzzlePosition.position,
                muzzlePosition.rotation,
                muzzlePosition
            );

            Destroy(flash, rangedData.muzzleFlashDuration);
        }
        protected override void PlayShotSound()
        {
            if (!(data is ShotgunWeaponData rangedData) || rangedData.shotSound == null)
                return;

            AudioSource.PlayClipAtPoint(rangedData.shotSound, transform.position);
        }
    }
}
