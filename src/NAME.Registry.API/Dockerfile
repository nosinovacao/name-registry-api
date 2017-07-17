FROM microsoft/dotnet:1.1.1-runtime

COPY . /app

WORKDIR /app

ENV Logging:NLogLogsDir=/var/log/app
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "NAME.Registry.API.dll"]