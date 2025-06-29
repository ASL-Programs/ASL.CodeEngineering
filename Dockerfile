# escape=`
# Use Windows SDK image to build the WPF solution
FROM mcr.microsoft.com/dotnet/sdk:7.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/ASL.CodeEngineering.App/ASL.CodeEngineering.App.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o /out

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0-windowsservercore-ltsc2022
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["ASL.CodeEngineering.App.exe"]
