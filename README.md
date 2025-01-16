# Sekur Client
Client for https://github.com/WhiteElement/sekur-server

Allows for automated backup of preconfigured folders and directories.
Client is best put to use with compiling an executeable and using windows "Aufgabenplanung" or Linux cron jobs in order to schedule the process.
Once all files and folders are collected a Zipping step is initiated before sending it to the provided server using following authentication.
Server ist best started by packageing a jar and running it: java -jar ./sekur-server.

### Uses API Key for Authentication:
* On startup provide a unique apikey
* provide the same on the server on startup
