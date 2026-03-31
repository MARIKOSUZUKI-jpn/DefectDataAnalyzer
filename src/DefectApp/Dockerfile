# 1. ビルド・環境（SDK）
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# プロジェクトファイルをコピーして復元（キャッシュ利用のため）
COPY *.csproj ./
RUN dotnet restore

# すべてのソースをコピーしてビルド
COPY . ./
RUN dotnet publish -c Release -o out

# 2. 実行用環境（軽量な Runtime）
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# アプリ起動
ENTRYPOINT ["dotnet", "DefectApp.dll"]