﻿using System;
using System.IO;
using Mono.Options;
using System.Diagnostics;
using KalliopeSync.Core.Full;

namespace KalliopeSync.Console
{
    class MainClass
    {
        static string accountName;
        static string accountKey;
        static string container;
        static string output;
        static int maxCount;
        static bool simulate = false;
        static bool showHelp;

        public static void Main(string[] args)
        {
            Logging.Logger.Init();
            var options = new OptionSet
                {
                    {
                        "m|maxcount=", "Maximum number of files to be downloaded",
                        (int m) => maxCount = m
                    },
                    { 
                        "n|accountName=", "Azure Service account name", 
                        s => accountName = s 
                    },
                    { 
                        "k|accountKey=", "Azure Service access key", 
                        (string o) => accountKey = o 
                    },
                    { 
                        "c|container=", "Container", 
                            (string c) => container = c 
                    },
                    {
                        "t|output=", "Output folder",
                            (string t) => output = t
                    },                
                    { 
                        "h|help", "Show this message and exit", 
                        h => showHelp = h != null 
                    },
                    {
                        "s|simulation=", "Simulate changes but don't upload or download files (s=true or t or 1, false/simulation off by default)",
                        (string s) => simulate = (s != null && (s == "true" || s == "t" || s == "1"))
                    }
                };

            try
            {
                options.Parse(args);
                if (showHelp)
                {
                    ShowHelp(options);
                }
                else
                {                    
                    int result = 0;
                    //int.TryParse(maxCount, out result);
                    result = maxCount;
                    System.Console.WriteLine(
                        string.Format("Options: \r\n 1-n: {0} \r\n 2-k: {1} \r\n 3-c: {2} \r\n 4-t: {3} \r\n 5-x: {4} \r 6-h: {5}", 
                            accountName,
                            accountKey,
                            container,
                            output,
                            result,
                            maxCount,
                            showHelp));
                    Downloader downloader = new Downloader(container, accountName, accountKey);
                    downloader.SimulationMode = simulate;
                    downloader.DownloadAll(output, result);
                    Uploader uploader = new Uploader(container, accountName, accountKey);
                    uploader.SimulationMode = simulate;
                    uploader.Upload(output);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error {0}", ex.Message);
                Logging.Logger.Error("ERROR: " + ex.Message, ex.InnerException);
            }
        }

        static void ShowHelp(OptionSet optionSet)
        {
            System.Console.WriteLine("Usage: kalliopesync -n <account name> -k <account key> -c <container>");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(System.Console.Out);
        }
    }
}
