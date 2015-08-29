CALL "%VS140COMNTOOLS%\VsDevCmd.bat"
MSBUILD build.proj -p:Configuration=Release
PAUSE