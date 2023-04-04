using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    public Slider healthBar;

    private bool invulnerable;

    private void SetInvulnerabilityWithDelay()
    {
        invulnerable = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy") || invulnerable) return;
        healthBar.value--;
        invulnerable = true;
        Task.Delay(1000).ContinueWith(t=> SetInvulnerabilityWithDelay());
    }
}
