FROM node:20-alpine AS giao-dien
WORKDIR /src/client
COPY client/package.json client/package-lock.json ./
RUN npm ci
COPY client/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS may-chu
WORKDIR /src
COPY QuanLyHanMucWeb.csproj ./
RUN dotnet restore QuanLyHanMucWeb.csproj
COPY . .
COPY --from=giao-dien /src/wwwroot ./wwwroot
RUN dotnet publish QuanLyHanMucWeb.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=may-chu /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "QuanLyHanMucWeb.dll"]
