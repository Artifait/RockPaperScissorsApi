# === 1. Runtime-����� �� ���� dotnet/runtime ===
ARG REPO=mcr.microsoft.com/dotnet/runtime
FROM $REPO:8.0.12-alpine3.20-amd64 AS runtime

# ������������� ASP.NET Core ������� (��� ultra-light final image)
ENV ASPNET_VERSION=8.0.12
RUN wget -O aspnetcore.tar.gz https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='a7d3bae2da7b4da946851d36196d41053593af4138d1ae020ce4b9b141c7e84d53446cb0891e127983abd5e7c011d7c9d2039227dca9409d6faeb6383583389a' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz

# === 2. ������ ������ ������� ===
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# �������� ������
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

# === 3. ��������� ����� �� ����������� runtime ===
FROM runtime
WORKDIR /app

# ����������� ����
EXPOSE 13080

# �������� ��������� ����������
COPY --from=build /app ./

# ��������� API
ENTRYPOINT ["dotnet", "RockPaperScissorsApi.dll"]
