using HardCard.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for TagReadEventArgsTest and is intended
    ///to contain all TagReadEventArgsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TagReadEventArgsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Time
        ///</summary>
        [TestMethod()]
        public void TimeTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            long expected = 0;
            long actual;
            target.Time = expected;
            actual = target.Time;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SignalStrenth
        ///</summary>
        [TestMethod()]
        public void SignalStrenthTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            float expected = 0F;
            float actual;
            target.SignalStrenth = expected;
            actual = target.SignalStrenth;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReferenceTime
        ///</summary>
        [TestMethod()]
        public void ReferenceTimeTest()
        {
            DateTime actual;
            actual = TagReadEventArgs.ReferenceTime;
            DateTime expected = new DateTime(1970,1,1);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            TagId expected = new TagId();
            TagId actual;
            target.ID = expected;
            actual = target.ID;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Frequency
        ///</summary>
        [TestMethod()]
        public void FrequencyTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            float expected = 0F;
            float actual;
            target.Frequency = expected;
            actual = target.Frequency;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DateTime
        ///</summary>
        [TestMethod()]
        public void DateTimeTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            DateTime expected = new DateTime();
            DateTime actual;
            target.DateTime = expected;
            actual = target.DateTime;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Antenna
        ///</summary>
        [TestMethod()]
        public void AntennaTest()
        {
            TagReadEventArgs target = new TagReadEventArgs();
            int expected = 0;
            int actual;
            target.Antenna = expected;
            actual = target.Antenna;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TagReadEventArgs Constructor
        ///</summary>
        [TestMethod()]
        public void TagReadEventArgsConstructorTest1()
        {
            TagId id = new TagId();
            id.Value = "foo";
            float frequency = 5609.3F;
            float signalStrength = 732.5F;
            int antenna = 3;
            long time = 0230124812040;
            TagReadEventArgs target = new TagReadEventArgs(id, frequency, signalStrength, antenna, time);
            Assert.AreEqual(id, target.ID);
            Assert.AreEqual(frequency, target.Frequency);
            Assert.AreEqual(signalStrength, target.SignalStrenth);
            Assert.AreEqual(antenna, target.Antenna);
            Assert.AreEqual(time, target.Time);
        }
    }
}
