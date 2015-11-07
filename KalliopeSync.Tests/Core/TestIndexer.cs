using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using KalliopeSync.Core;
using KalliopeSync.Core.Services;

namespace KalliopeSync.Tests.Core
{
    [TestFixture()]
    public class TestIndexer
    {
        string [] files = new string []
            {
                @"/home/sumitkm/mymusic/kalliope'sync.mp3",                                
                @"/home/sumitkm/myprojects/github/kalliopesync.suo",
                @"/home/sumitkm/myprojects/github/kalliopesync/.gitignore",
                @"/Downloads/en_windows_8.1_enterprise_with_update_x64_dvd_6054382.iso",
                @"/Downloads/DSC_3315.JPG",
                @"/home/sumitkm/.Xauthority",
                @"/home/sumitkm/.bash_history"
            };

        string [] fileIsIncludedList = new string[]
            {
                @"/home/sumitkm/mymusic/kalliope'sync.mp3",                                
                @"/Downloads/DSC_3315.JPG",
            };

        string [] fileIsExcludedList = new string[]
            {
                @"/home/sumitkm/myprojects/github/kalliopesync/.gitignore",                                
                @"/home/sumitkm/myprojects/github/kalliopesync.suo",
                @"/Downloads/en_windows_8.1_enterprise_with_update_x64_dvd_6054382.iso",
                @"/home/sumitkm/.Xauthority",
                @"/home/sumitkm/.bash_history"
            };


        string [] folders = new string[]
            {
                @"/home/sumitkm/Desktop/",
                @"/home/sumitkm/Documents/",
                @"/home/sumitkm/Downloads",
                @"/home/sumitkm/myprojects/github/",
                @"/home/sumitkm/myprojects/",
                @"/home/sumitkm/Pictures/",
                @"/home/sumitkm/Public/",
                @"/home/sumitkm/Templates/",
                @"/home/sumitkm/Videos/",
                @"/home/sumitkm/VirtualBox VMs/",
                @"/home/sumitkm/.atom/",
            };
        
        string [] folderIsIncludedList = new string[]
            {
                @"/home/sumitkm/Desktop",
                @"/home/sumitkm/Downloads",
                @"/home/sumitkm/myprojects",
                @"/home/sumitkm/Pictures",
                @"/home/sumitkm/Public",
                @"/home/sumitkm/Templates",
                @"/home/sumitkm/Videos",
            };

        string [] folderIsExcludedList = new string[]
            {
                @"/home/sumitkm/Documents",
                @"/home/sumitkm/myprojects/github",
                @"/home/sumitkm/VirtualBox VMs",
                @"/home/sumitkm/.atom/",                                
            };
        
        string [] patterns = new string[]
            {
                ".*/",
                "downloads/",
                "github/",
                "virtualBox vms/",
                "*.iso",
                "*.suo",
                "/.*"
            };

        [TestFixtureSetUp]
        public void SetupIndexerTest()
        {
            Console.WriteLine("----------------- START -----------------------");
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".kpsignore");
            System.IO.File.WriteAllLines(fileName, patterns);
        }

        [TestFixtureTearDown]
        public void TearDownIndexerTest()
        {
            Console.WriteLine("-------------------END------------------------");
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".kpsignore");
            System.IO.File.Delete(fileName);
        }

        [Test()]
        public void IsFileExcludedTest()
        {
            var idxr = new Indexer("","","",AppDomain.CurrentDomain.BaseDirectory);
            List<string> excludedList = new List<string>();   

            for (int j = 0; j < files.Length; j++)
            {
                string fileName = files[j];           
                if (!idxr.IsFileIncluded(fileName, patterns))
                {
                    Console.WriteLine("Excluded: {0}", fileName);
                    excludedList.Add(fileName);
                }
            }
            Assert.AreEqual(this.fileIsExcludedList.Length, excludedList.Count);
        }

        [Test()]
        public void IsFileIncludedTest()
        {
            var idxr = new Indexer("","","",AppDomain.CurrentDomain.BaseDirectory);
            List<string> includedList = new List<string>();   

            for (int j = 0; j < files.Length; j++)
            {
                string fileName = files[j];
                if (idxr.IsFileIncluded(fileName, patterns))
                {
                    Console.WriteLine("Included: {0}", fileName);

                    includedList.Add(fileName);
                }                    
            }

            Assert.AreEqual(this.fileIsIncludedList.Length, includedList.Count);
        }

        [Test()]
        public void IsFolderExcludedTest()
        {
            var idxr = new Indexer("","","",AppDomain.CurrentDomain.BaseDirectory);
            List<string> excludedList = new List<string>();   

            for (int j = 0; j < folders.Length; j++)
            {
                string fileName = folders[j];           
                if (!idxr.IsFolderIncluded(fileName, patterns))
                {
                    Console.WriteLine("Folder Excluded: {0}", fileName);
                    excludedList.Add(fileName);
                }
            }

            Assert.AreEqual(this.folderIsExcludedList.Length, excludedList.Count);
        }

        [Test()]
        public void IsFolderIncludedTest()
        {
            var idxr = new Indexer("","","",AppDomain.CurrentDomain.BaseDirectory);
            List<string> includedList = new List<string>();   

            for (int j = 0; j < folders.Length; j++)
            {
                string fileName = folders[j];
                if (idxr.IsFolderIncluded(fileName, patterns))
                {
                    Console.WriteLine("Folder Included: {0}", fileName);

                    includedList.Add(fileName);
                }
            }
            Assert.AreEqual(this.folderIsIncludedList.Length, includedList.Count);
        }

        [Test()]
        public void ReadPatternsFileTest()
        {
            var idxr = new Indexer("", "", "", AppDomain.CurrentDomain.BaseDirectory);
            var newPatterns = idxr.LoadPatterns(AppDomain.CurrentDomain.BaseDirectory);
            Assert.AreEqual(patterns.Length, newPatterns.Length);
        }

        [Test()]
        public void GetLocalRepositoryTest()
        {
            
        }
    }
}

