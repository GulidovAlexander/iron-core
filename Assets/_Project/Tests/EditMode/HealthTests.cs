using Game.Components.Health;
using Game.Scripts.Core.DataStructs;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class HealthTests
    {
        private const float MaxHealth = 100f;
        private const float HalfDamage = 50f;
        private const float OverKillDamage = 150f;
        
        private GameObject testObject;
        private HealthComponent health;

        [SetUp]
        public void Setup()
        {
            testObject = new GameObject();
            health = testObject.AddComponent<HealthComponent>();
            health.Initialize(MaxHealth);
        }
        
        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void IsDead_WhenHealthZero_ReturnsTrue()
        {
            health.TakeDamage(new DamageData(OverKillDamage, DamageType.Physical, null));
    
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void TakeDamage_FiresOnHealthChangedEvent()
        {
            float receivedCurrent = 0;
            float receivedMax = 0;
            var damage = new DamageData(30f, DamageType.Physical, null);
            health.OnHealthChanged += (current, max) => 
            {
                receivedCurrent = current;
                receivedMax = max;
            };
            
            health.TakeDamage(damage);
            
            Assert.AreEqual(70f, receivedCurrent);
            Assert.AreEqual(MaxHealth, receivedMax);
        }
        
        
        [Test]
        public void Heal_IncreasesHealth()
        {
            float receivedHealth = 0;
            health.TakeDamage(new DamageData(HalfDamage, DamageType.Physical, null));
            health.OnHealthChanged += (current, _) => receivedHealth = current;

            health.Heal(30f);

            Assert.AreEqual(80f, receivedHealth);
        }
        
        [Test]
        public void Heal_CannotExceedMaxHealth()
        {
            float receivedHealth = 0f;
            health.TakeDamage(new DamageData(30f, DamageType.Physical, null));
            health.OnHealthChanged += (current, _) => receivedHealth = current;

            health.Heal(MaxHealth);

            Assert.AreEqual(MaxHealth, receivedHealth);
        }
        
        [Test]
        public void MultipleDamageHits_WorkCorrectly()
        {
            float lastHealth = 0;
            health.OnHealthChanged += (current, _) => lastHealth = current;

            health.TakeDamage(new DamageData(30f, DamageType.Physical, null));
            health.TakeDamage(new DamageData(20f, DamageType.Physical, null));

            Assert.AreEqual(50f, lastHealth);
        }
        
        [Test]
        public void TakeDamage_AfterDeath_DoesNothing()
        {
            int deathCount = 0;
            health.OnDeath += () => deathCount++;

            health.TakeDamage(new DamageData(OverKillDamage, DamageType.Physical, null));
            health.TakeDamage(new DamageData(10f, DamageType.Physical, null));

            Assert.AreEqual(1, deathCount);
        }
        
        [Test]
        public void TakeDamage_WhenKilled_FiresOnDeath()
        {
            var deathFired = false;
            health.OnDeath += () => deathFired = true;

            health.TakeDamage(new DamageData(OverKillDamage, DamageType.Physical, null));

            Assert.IsTrue(deathFired);
        }
    }
}