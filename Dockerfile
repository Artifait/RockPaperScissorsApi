# === 1. Runtime-образ на базе dotnet/runtime ===
ARG REPO=mcr.microsoft.com/dotnet/runtime
FROM $REPO:8.0.12-alpine3.20-amd64 AS runtime

# Устанавливаем ASP.NET Core вручную (для ultra-light final image)
ENV ASPNET_VERSION=8.0.12
RUN wget -O aspnetcore.tar.gz https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='a7d3bae2da7b4da946851d36196d41053593af4138d1ae020ce4b9b141c7e84d53446cb0891e127983abd5e7c011d7c9d2039227dca9409d6faeb6383583389a' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# === 2. Стадия сборки проекта ===
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Копируем проект
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

# === 3. Финальный образ на облегчённом runtime ===
FROM runtime
WORKDIR /app

# Прокидываем порт
EXPOSE 13080

# Копируем собранное приложение
COPY --from=build /app ./

# Запускаем API
ENTRYPOINT ["dotnet", "RockPaperScissorsApi.dll"]
