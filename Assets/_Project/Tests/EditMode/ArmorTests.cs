using Game.Components.Health;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ArmorTests
    {
        private const float MaxArmor = 100f;
        private const float HalfArmor = 50f;

        private GameObject testObject;
        private ArmorComponent armor;

        [SetUp]
        public void Setup()
        {
            testObject = new GameObject();
            armor = testObject.AddComponent<ArmorComponent>();
            armor.Initialize(MaxArmor);
            armor.AddArmor(MaxArmor);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void HasArmor_WhenArmorZero_ReturnsFalse()
        {
            armor.Absorb(MaxArmor);

            Assert.IsFalse(armor.HasArmor);
        }

        [Test]
        public void Absorb_ReducesArmor()
        {
            armor.Absorb(30f);

            Assert.AreEqual(70f, armor.CurrentArmor);
        }

        [Test]
        public void Absorb_ReturnsRemainingDamage()
        {
            float remaining = armor.Absorb(30f);

            Assert.AreEqual(0f, remaining);
        }

        [Test]
        public void Absorb_WhenDamageExceedsArmor_ReturnsRemainder()
        {
            armor.Absorb(MaxArmor - HalfArmor);

            float remaining = armor.Absorb(80f);

            Assert.AreEqual(30f, remaining);
        }

        [Test]
        public void Absorb_WhenNoArmor_ReturnsFullDamage()
        {
            armor.Absorb(MaxArmor);

            float remaining = armor.Absorb(50f);

            Assert.AreEqual(50f, remaining);
        }

        [Test]
        public void Absorb_FiresOnArmorChangedEvent()
        {
            float receivedCurrent = 0;
            armor.OnArmorChanged += (current, _) => receivedCurrent = current;

            armor.Absorb(30f);

            Assert.AreEqual(70f, receivedCurrent);
        }

        [Test]
        public void AddArmor_IncreasesArmor()
        {
            armor.Absorb(HalfArmor);
            float receivedArmor = 0;
            armor.OnArmorChanged += (current, _) => receivedArmor = current;

            armor.AddArmor(20f);

            Assert.AreEqual(70f, receivedArmor);
        }

        [Test]
        public void AddArmor_CannotExceedMaxArmor()
        {
            float receivedArmor = 0;
            armor.OnArmorChanged += (current, _) => receivedArmor = current;

            armor.AddArmor(MaxArmor);

            Assert.AreEqual(MaxArmor, receivedArmor);
        }
    }
}