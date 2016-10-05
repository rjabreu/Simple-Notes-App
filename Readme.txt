App is built using .NET framework so IIS web app is required to run this,
or if you are using Visual Studio 2013+ just run in debug mode and VS will load an instance of IIS Epxress.

App creates a folder under "C:\" to store list of notes created. Ideally this would be saved on server folder.
however, I chose this way to mitigate issues with folder access rights.