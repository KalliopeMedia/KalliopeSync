using NUnit.Framework;
using System;
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

        string [] isIncluded = new string[]
            {
                @"/Downloads/DSC_3315.JPG"
            };

        string [] isExcluded = new string[]
            {
                @"/home/sumitkm/myprojects/github/kalliopesync.suo",
                @"/home/sumitkm/myprojects/github/kalliopesync/.gitignore",
                @"/Downloads/en_windows_8.1_enterprise_with_update_x64_dvd_6054382.iso",
            };
            

        string [] patterns = new string[]
            {
                "#build",
                "[Oo]bj/",
                "[Bb]in/",
                "packages/",
                "TestResults/",
                "github/",
                "# globs",
                "Makefile.in",
                "*.DS_Store",
                "*.sln.cache",
                "*.suo",
                "*.cache",
                "*.pidb",
                "*.iso",
                "*.userprefs",
                "*.usertasks",
                ".gitignore",
                "config.log",
                "config.make",
                "config.status",
                "aclocal.m4",
                "install-sh",
                "autom4te.cache/",
                "*.user"
            };

        public TestIndexer()
        {
            Console.WriteLine("----------------------------------------------");
        }

        [Test()]
        public void IsFileExcludedTest()
        {
            List<string> excludedList = new List<string>();   

            for (int j = 0; j < files.Length; j++)
            {
                string fileName = files[j];
            
                for (int i = 0; i < patterns.Length; i++)
                {
                    if (fileName.Like(patterns[i]))
                    {
                        Console.WriteLine("Excluded {0} - {1}", fileName, patterns[i]);
                        excludedList.Add(fileName);
                        break;
                    }
                }
            }

            Assert.AreEqual(this.isExcluded.Length, excludedList.Count);
        }

        [Test()]
        public void IsFileIncludedTest()
        {
            List<string> includedList = new List<string>();   

            for (int j = 0; j < files.Length; j++)
            {
                string fileName = files[j];
                bool isExcluded = false;
                for (int i = 0; i < patterns.Length; i++)
                {
                    if (fileName.Like(patterns[i]))
                    {
                        isExcluded = true;
                        break;
                    }
                }
                if (!isExcluded)
                {
                    Console.WriteLine("Included {0}", fileName);

                    includedList.Add(fileName);
                }
            }

            Assert.AreEqual(this.isIncluded.Length, includedList.Count);
        }

    }
}

