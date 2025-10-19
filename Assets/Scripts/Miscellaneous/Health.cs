using UnityEngine;
using UnityEngine.SceneManagement;

namespace Miscellaneous
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth;

        public void TakeDamage(float damage)
        {
            maxHealth -= damage;
            if(maxHealth <= 0)
            {
                if(gameObject.CompareTag("Player"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
