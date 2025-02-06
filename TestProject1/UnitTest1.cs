using NUnit.Framework;

namespace TestProject1
{
    public class NUnitTests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("==== Start of Testing ====");
        }

        [Test]
        public void IsEqual()
        {
            Assert.That(1 == 2, Is.True);
        }

        [Test]
        public void AreEqualNumbers()
        {
            Assert.That(2, Is.EqualTo(1));
        }

        [Test]
        public void IsMoreThanAndLessThan()
        {
            var testValue = 100;
            Assert.That(testValue, Is.GreaterThan(0).And.LessThanOrEqualTo(100));
        }
    }
}