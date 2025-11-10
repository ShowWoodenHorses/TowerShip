using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interface
{
    public interface IDamagable
    {
        public void TakeDamage(int damage);

        public bool IsDiedEnemy();
        public bool CheckDiedEnemy();

        public void SetCheckDiedEnemy();
    }
}