REM SET MSBUILD="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
REM SET MSBUILD="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
REM SET MSBUILD="C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe"
SET MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\amd64\MSBuild.exe"
SET SOLUTION_FILE=evtailNet.sln

%MSBUILD% %SOLUTION_FILE% /m %*
