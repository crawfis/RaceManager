using HardCard.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for TagPassingTest and is intended
    ///to contain all TagPassingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TagPassingTest
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
        ///A test for TagPassing Constructor
        ///</summary>
        [TestMethod()]
        public void TagPassingConstructorTest()
        {
            TagPassing target = new TagPassing("Tag Passing");
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddReader
        ///</summary>
        TagPassing testPassing;
        TagReadEventArgs tagArgs;
        [TestMethod()]
        public void AddReaderTest()
        {
            TagPassing target = new TagPassing("Tag Passing");
            testPassing = target;
            NetworkListener rfidReader = new NetworkListener("TestListener");
            target.AddPublisher(rfidReader);
            tagArgs = new TagReadEventArgs();
            //rfidReader.TestReceivePacket(tagArgs);
        }
        void TagPassedEventTest(object sender, TagReadEventArgs args)
        {
            Assert.AreEqual(sender, testPassing);
            Assert.AreEqual(args, tagArgs);
            Assert.IsTrue(TagInfo.AreEqual(args.TagInfo, tagArgs.TagInfo));
        }

        /// <summary>
        ///A test for AddTag
        ///</summary>
        [TestMethod()]
        public void AddTagTest()
        {
            TagPassing target = new TagPassing(); // TODO: Initialize to an appropriate value
            TagId tagId = new TagId(); // TODO: Initialize to an appropriate value
            target.AddTag(tagId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
