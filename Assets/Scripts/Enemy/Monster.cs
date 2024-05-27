using UnityEngine;
using Fusion;
using Fire;
//using Goldmetal.UndeadSurvivor;

namespace Enemy
{
    public class Monster : NetworkBehaviour
    {
        public float health = 100f;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void TakeDamageRpc(float damage, PlayerRef attacker)
        {
            Debug.Log($"TakeDamageRpc called by {attacker}, damage: {damage}");
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                ShowHitEffectRpc();
            }
        }

        public void TakeDamage(float damage, PlayerRef attacker)
        {
            Debug.Log($"TakeDamage called by {attacker}, damage: {damage}");
            if (Object.HasStateAuthority)
            {
                TakeDamageRpc(damage, attacker);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void ShowHitEffectRpc()
        {
            ShowHitEffect();
        }

        private void ShowHitEffect()
        {
            Debug.Log("ShowHitEffect called.");
            animator.SetTrigger("Hit");
        }

        private void Die()
        {
            Debug.Log("Monster died.");
            if (Object != null && Runner != null)
            {
                Runner.Despawn(Object);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("OnTriggerEnter2D called.");
            if (collision.CompareTag("Bullet"))
            {
                Bullet bullet = collision.GetComponent<Bullet>();
                if (bullet != null)
                {
                    Debug.Log("Monster was hit by a bullet!");
                    TakeDamage(bullet.damage, bullet.Object.InputAuthority);
                    if (bullet.Object != null && bullet.Runner != null)
                    {
                        Runner.Despawn(bullet.Object);
                    }
                }
            }
        }
    }
}