[![Build Status](https://travis-ci.org/KalliopeMedia/KalliopeSync.svg)](https://travis-ci.org/KalliopeMedia/KalliopeSync)

# KalliopeSync

Kalliope Sync is an open source sync project that helps you sync local content to an Azure Blob store.
You need to have your own Azure Account and Blob storage subscription to use it.
###NOTE: Work in progress, doesn't actually sync anything yet. At best it will copy new files from local filesystem to Azure

##Usage
```
Usage (Linux): 

mono kalliopesync.exe -n <account name> -k <account key> -c <container>

Usage (Windows): 

kalliopesync -n <account name> -k <account key> -c <container>

Options:
  -m, --maxcount=VALUE       Maximum number of files to be downloaded
  -n, --accountName=VALUE    Azure Service account name
  -k, --accountKey=VALUE     Azure Service access key
  -c, --container=VALUE      Container
  -t, --output=VALUE         Output folder
  -s, --simulation=VALUE     Simulate changes but don't upload or download
                               files (s=true or t or 1, false/simulation off by
                               default)
  -f, --fullthrottle=VALUE   By default true, uploads as fast as it can. If
                               false, breaks down files in 1024byte chunks and
                               give 500 ms break before uploading each chunk
  -h, --help                 Show this message and exit

```
