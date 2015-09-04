[![Build Status](https://travis-ci.org/KalliopeMedia/KalliopeSync.svg)](https://travis-ci.org/KalliopeMedia/KalliopeSync)

# KalliopeSync

Kalliope Sync is an open source sync project that helps you sync local content to an Azure Blob store.
You need to have your own Azure Account and Blob storage subscription to use it.

##Kalliope.Console
The console client that invokes the Core sync functionality.

##Usage
```
Usage: kalliopesync -n <account name> -k <account key> -c <container>

Options:
  -m, --maxcount=VALUE       Maximum number of files to be downloaded
  -n, --accountName=VALUE    Azure Service account name
  -k, --accountKey=VALUE     Azure Service access key
  -c, --container=VALUE      Container
  -t, --output=VALUE         Output folder
  -h, --help                 Show this message and exit
  -s, --simulation=VALUE     Simulate changes but don't upload or download
                               files (s=true or t or 1, false/simulation off by
                               default)
```
