# BuildHelper

_Visual Studio 2013/2015 extension used to stop/start/restart services and processes before/after build._

Constantly getting errors like the ones below? </br>
_Error		Could not copy "obj\Release\Test.dll" to "..\bin\Release\Test.dll". Exceeded retry count of 10. Failed._	
_Error		Unable to copy file "obj\Release\Test.dll" to "..\bin\Release\Test.dll". The process cannot access the file '..\bin\Release\Test.dll' because it is being used by another process_

Constantly having to worry if your services/processes using the dlls are stoped before building? Keep forgetting to restart them?

#### Then this extension is for you!

Configure this extension one time (see options window below) and it will make you more productive and you will not get this kind of errors again.

To configure:
- Add solution names for BuildHelper to track.
- Assign processes and services for solution names to stop/start before/after build. If you add more than one use ';' separator.
- Check "restart" checkboxes if you want them to be automatically restarted after a succesful build
- Make sure that you have required permissions to kill processes and stop services - run Visual Studio as administrator.


![BuildHelper options UI](https://github.com/nmklotas/BuildHelper/blob/master/Documents/UI.png "BuildHelper UI")

Download it from Visual Studio Gallery:</br>
https://visualstudiogallery.msdn.microsoft.com/6288279e-556d-4d6d-858a-4f10c74511d1
