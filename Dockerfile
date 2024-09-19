FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /App
EXPOSE 8080 8443
COPY /App .

ENTRYPOINT ["dotnet", "PantryPad.dll"]
