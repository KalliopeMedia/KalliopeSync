using System;
using System.IO;
using Mono.Options;
using System.Diagnostics;
using KalliopeSync.Core.Services;

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
        static bool fullThrottle = false;
        static string moveall = "";

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
                        "c|container=", "Azure Blob Container name where files will be uploaded", 
                            (string c) => container = c 
                    },
                    {
                        "t|output=", "Sync folder",
                            (string t) => output = t
                    },
                    {
                        "s|simulation=", "Simulate changes but don't upload or download files (s=true/t/1 OR false/f/0. Simulation off by default)",
                        (string s) => simulate = (s != null && (s == "true" || s == "t" || s == "1"))
                    },
                    {
                        "f|fullthrottle=", "By default true, uploads as fast as it can. If false, breaks down files in 1024byte chunks and give 100 ms break before uploading each chunk",
                        (string f) => fullThrottle = (f != null && (f == "true" || f == "t" || f == "1"))
                    },
                    {
                        "v|moveall=", "Moves all files in root folder to specified folder",
                        (string v) => moveall = v
                    },
                    { 
                        "h|help", "Show this message and exit", 
                        h => showHelp = h != null 
                    }
                };

            var sessionId = DateTime.Now.Ticks;
            try
            {
                options.Parse(args);
                if (showHelp || args.Length == 0)
                {
                    ShowHelp(options);
                }
                else
                {                    
                    int result = 0;
                    //int.TryParse(maxCount, out result);
                    result = maxCount;
                    System.Console.WriteLine(
                        string.Format("Options: \r\n 1-n: {0} \r\n 2-k: {1} \r\n 3-c: {2} \r\n 4-t: {3} \r\n 5-m: {4} \r 6-f: {5}, \r 7-v: {6}, \r 8-h: {7}", 
                            accountName,
                            accountKey,
                            container,
                            output,
                            maxCount,
                            fullThrottle.ToString(),
                            moveall.ToString(),
                            showHelp.ToString()));
                    Logging.Logger.Info(string.Format("Starting new session id: {0} at {1} :---------------------", sessionId, DateTime.Now.ToLongDateString()));
                    Downloader downloader = new Downloader(container, accountName, accountKey);
                    downloader.SimulationMode = simulate;
                    downloader.DownloadAll(output, result);

                    System.Console.WriteLine("Indexing");
                    Indexer differ = new Indexer(container, accountName, accountKey, output);
                    System.Console.WriteLine("Indexing Complete");

                    var uploadList = differ.CreateUploadList(output);

                    Uploader uploader = new Uploader(container, accountName, accountKey);
                    uploader.SimulationMode = simulate;
                    uploader.FullThrottle = fullThrottle;
                    uploader.Upload(output, uploadList);

                    if(!string.IsNullOrEmpty(moveall))
                    {
                        VirtualFileSystem vfs = new VirtualFileSystem(accountName, accountKey, container);
                        vfs.Move(moveall);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error {0}", ex.Message);
                Logging.Logger.Error(ex.Message, ex);
            }
            finally
            {
                Logging.Logger.Info(string.Format("Ending session {0} at {1} :---------------------", sessionId, DateTime.Now.ToLongDateString()));
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
