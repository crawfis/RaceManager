using HardCard.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for NullPassingStrategyTest and is intended
    ///to contain all NullPassingStrategyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NullPassingStrategyTest
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
        ///A test for NullPassingStrategy Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HardCardTests.exe")]
        public void NullPassingStrategyConstructorTest()
        {
            NullPassingStrategy_Accessor target = new NullPassingStrategy_Accessor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for HandlePassing
        ///</summary>
        [TestMethod()]
        public void HandlePassingTest()
        {
            NullPassingStrategy_Accessor target = new NullPassingStrategy_Accessor(); // TODO: Initialize to an appropriate value
            TagReadEventArgs tagInfo = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.HandlePassing(tagInfo);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HardCardTests.exe")]
        public void InstanceTest()
        {
            NullPassingStrategy_Accessor expected = null; // TODO: Initialize to an appropriate value
            NullPassingStrategy_Accessor actual;
            NullPassingStrategy_Accessor.Instance = expected;
            actual = NullPassingStrategy_Accessor.Instance;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Passing
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HardCardTests.exe")]
        public void PassingTest()
        {
            NullPassingStrategy_Accessor target = new NullPassingStrategy_Accessor(); // TODO: Initialize to an appropriate value
            TagReadEventArgs expected = null; // TODO: Initialize to an appropriate value
            TagReadEventArgs actual;
            target.Passing = expected;
            actual = target.Passing;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PreviousPassing
        ///</summary>
        [TestMethod()]
        public void PreviousPassingTest()
        {
            NullPassingStrategy_Accessor target = new NullPassingStrategy_Accessor(); // TODO: Initialize to an appropriate value
            TagReadEventArgs expected = null; // TODO: Initialize to an appropriate value
            TagReadEventArgs actual;
            target.PreviousPassing = expected;
            actual = target.PreviousPassing;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
