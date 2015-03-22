# epidigitalogy
This is a short readme on the minimum requirements for using Digital Disease Tracking Application

The application has been tested in the following environment:

Operating System: Windows 2012 Standard Edition and Windows 7
Database: Microsoft SQL 2012
Web Server: Microsoft IIS 8, .NET 4.5

In order to install this web site, perform the following:

-Create a new web site in IIS
-Point the web site to the WebApp/Epidigitalogy directory that was unzipped.
-Make sure in the App Pools section the pool for your new web site is set to use .NET 4.0.  There's no option for 4.5.

If you just installed IIS, you may need to reinstall .NET 4.5 so it integrates properly into IIS.
If you need to get .NET 4.5, you can download it here:  http://www.microsoft.com/en-us/download/confirmation.aspx?id=42642
Performing a "repair" should get it working properly.

Customize the database string in web.config. Currently configured with following settings:

	ServerName: JOHNSNOW
	Database Name: SEM5
	SQL User: epi 
	SQL Password: password

Feel free to change for your environment.

The SQL user used must have the "db_datareader" role on the "sem5" (Symantec) database.  To do this:

-Connect to the database using Sql Server Management Studio (SSMS) with "sa" or another database administrator user.
-Under the database, go to Security/Logins
-Go to the Properties of the User in the right-click menu
-Select the User Mapping page on the left
-Check the database (default is "SEM5") and make sure "db_datareader" is selected on the bottom

Go to your new web site in a web browser.

