# Use the latest Windows Server Core 2022 image.
FROM mcr.microsoft.com/dotnet/runtime:6.0.20-windowsservercore-ltsc2022
    
ADD https://aka.ms/vs/17/release/vc_redist.x64.exe vc_redist.x64.exe
RUN vc_redist.x64.exe /install /quiet /norestart /log vc_redist.log   

# Restore the default Windows shell for correct batch processing.
SHELL ["cmd", "/S", "/C"]

WORKDIR /x64
COPY x64/ .

RUN ((echo start c:\x64\Release\net6.0\swaptoken.exe) && (echo timeout 2) && echo c:\x64\Release\callgettoken.exe) > start.cmd


CMD ["start.cmd"]
