using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using KalliopeSync.Core.Full;
using KalliopeSync.Db;

namespace KalliopeSync.Tests
{
    [TestFixture()]
    public class TestDbServices
    {
        public TestDbServices()
        {
        }

        [TestFixtureSetUp]
        public void SetupIndexerTest()
        {
        }

        [TestFixtureTearDown]
        public void TearDownIndexerTest()
        {
        }

        [Test()]
        public void CreateDbUsingProviderTest()
        {
            string name = DateTime.Now.Ticks.ToString();
            var provider = new Provider(name);

            bool exists = provider.VerifyDb();

            Assert.AreEqual(true, exists);
            File.Delete(name);
        }

        [Test()]
        public void CreateDbInFolderUsingProviderTest()
        {
            string fileName = DateTime.Now.Ticks.ToString() + ".kpsdb";
            string folderName = DateTime.Now.Ticks.ToString();
            var provider = new Provider(fileName, folderName);

            var fullFileName = Path.Combine(folderName, fileName);

            bool exists = provider.VerifyDb();

            Assert.AreEqual(true, exists);
            File.Delete(fullFileName);
            Directory.Delete(folderName, true);
        }
    }
}

