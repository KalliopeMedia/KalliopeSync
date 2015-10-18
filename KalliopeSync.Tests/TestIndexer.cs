using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using KalliopeSync.Core.Full;

namespace KalliopeSync.Tests
{
    [TestFixture()]
    public class TestIndexer
    {
        string [] files = new string []
            {
                @"/home/sumitkm/myprojects/github/kalliopesync.suo",
                @"/home/sumitkm/myprojects/github/kalliopesync/.gitignore",
                @"/Downloads/en_windows_8.1_enterprise_with_update_x64_dvd_6054382.iso",
                @"/Downloads/DSC_3315.JPG"
            };

        string [] fileIsIncludedList = new string[]
            {
                @"/Downloads/DSC_3315.JPG",
                @"/home/sumitkm/myprojects/github/kalliopesync/.gitignore",
            };

        string [] fileIsExcludedList = new string[]
            {
                @"/home/sumitkm/myprojects/github/kalliopesync.suo",
                @"/Downloads/en_windows_8.1_enterprise_with_update_x64_dvd_6054382.iso",
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
            };
        
        string [] folderIsIncludedList = new string[]
            {
                @"/home/sumitkm/Desktop",
                @"/home/sumitkm/Documents",
                @"/home/sumitkm/Downloads",
                @"/home/sumitkm/myprojects",
                @"/home/sumitkm/Pictures",
                @"/home/sumitkm/Public",
                @"/home/sumitkm/Templates",
                @"/home/sumitkm/Videos",
            };

        string [] folderIsExcludedList = new string[]
            {
                @"/home/sumitkm/myprojects/github",
                @"/home/sumitkm/VirtualBox VMs",
            };
        
        string [] patterns = new string[]
            {
                "downloads/",
                "github/",
                "virtualBox vms/",
                "*.iso",
                "*.suo"
            };

        [TestFixtureSetUp]
        public void SetupIndexerTest()
        {
            Console.WriteLine("----------------------------------------------");

        }

        [Test()]
        public void IsFileExcludedTest()
        {
            var idxr = new Indexer("","","","");
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
            var idxr = new Indexer("","","","");
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
            var idxr = new Indexer("","","","");
            List<string> excludedList = new List<string>();   

            for (int j = 0; j < folders.Length; j++)
            {
                string fileName = folders[j];           
                if (!idxr.IsFileIncluded(fileName, patterns))
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
            var idxr = new Indexer("","","","");
            List<string> includedList = new List<string>();   

            for (int j = 0; j < folders.Length; j++)
            {
                string fileName = folders[j];
                if (idxr.IsFileIncluded(fileName, patterns))
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
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".kpsignore");
            System.IO.File.WriteAllLines(fileName, patterns);
            var idxr = new Indexer("", "", "", "");
            var newPatterns = idxr.LoadPatterns(AppDomain.CurrentDomain.BaseDirectory);
            System.IO.File.Delete(fileName);
            Assert.AreEqual(patterns.Length, newPatterns.Length);
                
        }

        [Test()]
        public void GetLocalRepositoryTest()
        {
            
        }
    }
}

