using HardCard.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for NullListenerTest and is intended
    ///to contain all NullListenerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NullListenerTest
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
        ///A test for NullListener Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HardCardTests.exe")]
        public void NullListenerConstructorTest()
        {
            NullListener_Accessor target = new NullListener_Accessor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for IgnoreEvent
        ///</summary>
        [TestMethod()]
        public void IgnoreEventTest()
        {
            NullListener_Accessor target = new NullListener_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs args = null; // TODO: Initialize to an appropriate value
            target.IgnoreEvent(sender, args);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HardCardTests.exe")]
        public void InstanceTest()
        {
            NullListener expected = null; // TODO: Initialize to an appropriate value
            NullListener actual;
            NullListener_Accessor.Instance = expected;
            actual = NullListener_Accessor.Instance;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
