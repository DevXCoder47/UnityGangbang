using UnityEngine;

namespace NPC
{
    public class Rotation : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private float rotationSpeed;

        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        void Update()
        {
            if (target == null) return;

            // Вектор до цели (с учётом высоты)
            Vector3 direction = target.transform.position - transform.position;

            if (direction != Vector3.zero)
            {
                // Поворот в сторону цели
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Устанавливаем полный плавный поворот (и по Y, и по X)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
