# NNews - Guia de Publicação de Pacotes NuGet

Este documento fornece instruções para criar e publicar os pacotes NuGet `NNews.DTO` e `NNews.ACL`.

## ?? Pacotes Disponíveis

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| **NNews.DTO** | 2.0.0 | Data Transfer Objects - Modelos de dados para comunicação API |
| **NNews.ACL** | 2.0.0 | Anti-Corruption Layer - Cliente HTTP tipado para consumir NNews API |

## ?? Pré-requisitos

1. **.NET SDK 8.0+** instalado
2. **Conta NuGet.org** (ou servidor NuGet privado)
3. **API Key do NuGet** configurada

### Obter API Key do NuGet.org

1. Acesse [nuget.org](https://www.nuget.org)
2. Faça login na sua conta
3. Vá em **Account Settings** ? **API Keys**
4. Clique em **Create**
5. Configure:
   - **Key Name**: NNews Packages
   - **Select Scopes**: Push
   - **Select Packages**: All Packages
   - **Expiration**: 365 days
6. Copie a API Key gerada

### Configurar API Key Localmente

```powershell
# Windows
dotnet nuget push -k YOUR_API_KEY_HERE --source https://api.nuget.org/v3/index.json

# Ou adicionar a API key globalmente
dotnet nuget setapikey YOUR_API_KEY_HERE --source https://api.nuget.org/v3/index.json
```

## ??? Build dos Pacotes

### Opção 1: Build Individual

```powershell
# Build NNews.DTO
cd C:\repos\NNews\NNews.DTO
dotnet pack -c Release -o ..\packages

# Build NNews.ACL
cd C:\repos\NNews\NNews.ACL
dotnet pack -c Release -o ..\packages
```

### Opção 2: Build Todos de Uma Vez

```powershell
# Da raiz do repositório
cd C:\repos\NNews

# Criar pasta para pacotes
mkdir packages

# Build DTO
dotnet pack NNews.DTO\NNews.DTO.csproj -c Release -o packages

# Build ACL (depende do DTO)
dotnet pack NNews.ACL\NNews.ACL.csproj -c Release -o packages
```

### Opção 3: Script PowerShell

Crie um arquivo `build-packages.ps1`:

```powershell
# build-packages.ps1
param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "packages"
)

Write-Host "Building NNews NuGet packages..." -ForegroundColor Green

# Limpar pasta de output
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath\* -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

# Build NNews.DTO
Write-Host "`nBuilding NNews.DTO..." -ForegroundColor Cyan
dotnet pack NNews.DTO\NNews.DTO.csproj `
    -c $Configuration `
    -o $OutputPath `
    --include-symbols `
    --include-source `
    /p:SymbolPackageFormat=snupkg

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to build NNews.DTO" -ForegroundColor Red
    exit 1
}

# Build NNews.ACL
Write-Host "`nBuilding NNews.ACL..." -ForegroundColor Cyan
dotnet pack NNews.ACL\NNews.ACL.csproj `
    -c $Configuration `
    -o $OutputPath `
    --include-symbols `
    --include-source `
    /p:SymbolPackageFormat=snupkg

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to build NNews.ACL" -ForegroundColor Red
    exit 1
}

Write-Host "`nPackages built successfully!" -ForegroundColor Green
Write-Host "Location: $OutputPath" -ForegroundColor Yellow
Get-ChildItem $OutputPath -Filter *.nupkg | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}
```

Executar:

```powershell
.\build-packages.ps1
```

## ?? Publicar Pacotes

### Opção 1: Publicação Manual

```powershell
cd C:\repos\NNews\packages

# Publicar NNews.DTO
dotnet nuget push NNews.DTO.2.0.0.nupkg `
    --api-key YOUR_API_KEY `
    --source https://api.nuget.org/v3/index.json

# Publicar NNews.ACL
dotnet nuget push NNews.ACL.2.0.0.nupkg `
    --api-key YOUR_API_KEY `
    --source https://api.nuget.org/v3/index.json
```

### Opção 2: Script PowerShell de Publicação

Crie um arquivo `publish-packages.ps1`:

```powershell
# publish-packages.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$ApiKey,
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [string]$PackagesPath = "packages"
)

Write-Host "Publishing NNews NuGet packages..." -ForegroundColor Green

# Verificar se a pasta existe
if (-not (Test-Path $PackagesPath)) {
    Write-Host "Packages folder not found: $PackagesPath" -ForegroundColor Red
    Write-Host "Run build-packages.ps1 first" -ForegroundColor Yellow
    exit 1
}

# Buscar todos os .nupkg (exceto símbolos)
$packages = Get-ChildItem $PackagesPath -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.symbols.nupkg" }

if ($packages.Count -eq 0) {
    Write-Host "No packages found in $PackagesPath" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($packages.Count) package(s) to publish" -ForegroundColor Cyan

foreach ($package in $packages) {
    Write-Host "`nPublishing $($package.Name)..." -ForegroundColor Yellow
    
    dotnet nuget push $package.FullName `
        --api-key $ApiKey `
        --source $Source `
        --skip-duplicate
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Published successfully" -ForegroundColor Green
    } else {
        Write-Host "  ? Failed to publish" -ForegroundColor Red
    }
}

Write-Host "`nDone!" -ForegroundColor Green
```

Executar:

```powershell
.\publish-packages.ps1 -ApiKey YOUR_API_KEY
```

### Opção 3: Publicar para Feed Privado

```powershell
# Adicionar source privado
dotnet nuget add source https://your-private-feed.com/nuget `
    --name PrivateFeed `
    --username your-username `
    --password your-password

# Publicar
dotnet nuget push NNews.DTO.2.0.0.nupkg `
    --source PrivateFeed
```

## ?? Verificar Pacotes

### Inspecionar Conteúdo do Pacote

```powershell
# Listar conteúdo
dotnet nuget list package NNews.DTO --source https://api.nuget.org/v3/index.json

# Baixar e inspecionar
nuget.exe install NNews.DTO -OutputDirectory temp -NoCache
```

### Validar Pacote Antes de Publicar

```powershell
# Verificar se README está incluído
Expand-Archive packages\NNews.DTO.2.0.0.nupkg -DestinationPath temp\DTO
Get-ChildItem temp\DTO -Recurse

# Verificar metadados
nuget.exe spec packages\NNews.DTO.2.0.0.nupkg
```

## ?? Pipeline CI/CD (GitHub Actions)

Crie `.github/workflows/publish-nuget.yml`:

```yaml
name: Publish NuGet Packages

on:
  push:
    tags:
      - 'v*'  # Trigger on version tags (e.g., v2.0.0)

jobs:
  build-and-publish:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Pack NNews.DTO
      run: dotnet pack NNews.DTO/NNews.DTO.csproj -c Release -o packages --include-symbols --include-source /p:SymbolPackageFormat=snupkg
    
    - name: Pack NNews.ACL
      run: dotnet pack NNews.ACL/NNews.ACL.csproj -c Release -o packages --include-symbols --include-source /p:SymbolPackageFormat=snupkg
    
    - name: Publish to NuGet
      run: |
        dotnet nuget push packages/*.nupkg \
          --api-key ${{ secrets.NUGET_API_KEY }} \
          --source https://api.nuget.org/v3/index.json \
          --skip-duplicate
      env:
        NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
    
    - name: Create GitHub Release
      uses: actions/create-release@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        tag_name: ${{ github.ref }}
        release_name: Release ${{ github.ref }}
        draft: false
        prerelease: false
```

Configurar Secret no GitHub:
1. Vá em **Settings** ? **Secrets and variables** ? **Actions**
2. Clique em **New repository secret**
3. Name: `NUGET_API_KEY`
4. Value: Sua API Key do NuGet

## ?? Checklist de Publicação

Antes de publicar, verifique:

- [ ] Versão atualizada nos `.csproj`
- [ ] README.md atualizado com changelog
- [ ] Documentação XML gerada (`GenerateDocumentationFile=true`)
- [ ] Testes passando
- [ ] Build Release sem warnings
- [ ] README incluído no pacote
- [ ] Dependências corretas listadas
- [ ] Licença MIT especificada
- [ ] Tags apropriadas configuradas
- [ ] URL do repositório correta

## ?? Atualizar Versão

Edite os arquivos `.csproj`:

```xml
<PropertyGroup>
  <Version>2.1.0</Version>  <!-- Atualizar aqui -->
</PropertyGroup>
```

**Versionamento Semântico:**
- **MAJOR** (2.0.0): Breaking changes
- **MINOR** (2.1.0): Novas features (backward compatible)
- **PATCH** (2.0.1): Bug fixes

## ?? Testar Pacotes Localmente

```powershell
# Criar pasta de feed local
mkdir C:\local-nuget

# Adicionar como source
dotnet nuget add source C:\local-nuget --name LocalFeed

# Copiar pacotes
Copy-Item packages\*.nupkg C:\local-nuget\

# Criar projeto de teste
mkdir test-project
cd test-project
dotnet new console

# Instalar pacote local
dotnet add package NNews.DTO --source C:\local-nuget
dotnet add package NNews.ACL --source C:\local-nuget

# Testar
dotnet run
```

## ?? Monitorar Pacotes

### Ver Downloads

Acesse: https://www.nuget.org/packages/NNews.DTO

### Estatísticas

```powershell
# Via API
$response = Invoke-RestMethod -Uri "https://api.nuget.org/v3/registration5-semver1/nnews.dto/index.json"
$response.items[0].items | Select-Object -Last 1
```

## ??? Troubleshooting

### Erro: "Package already exists"

```powershell
# Use --skip-duplicate
dotnet nuget push package.nupkg --skip-duplicate
```

### Erro: "Invalid API Key"

```powershell
# Re-configurar API key
dotnet nuget remove source https://api.nuget.org/v3/index.json
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
dotnet nuget setapikey YOUR_NEW_KEY --source nuget.org
```

### Erro: "Package validation failed"

Verifique:
1. README.md existe no projeto
2. Licença está especificada
3. Não há símbolos especiais no nome do pacote

## ?? Recursos Adicionais

- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [Creating NuGet Packages](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package)
- [Publishing Packages](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Package Versioning](https://docs.microsoft.com/en-us/nuget/concepts/package-versioning)

## ?? Próximos Passos

Após publicação:

1. ? Verificar se pacotes aparecem em nuget.org (pode levar alguns minutos)
2. ? Testar instalação em projeto novo
3. ? Atualizar documentação do repositório
4. ? Anunciar nova versão no README principal
5. ? Criar GitHub Release com notas de versão
6. ? Atualizar projetos dependentes

---

**Última atualização:** 2024-01-15  
**Versão atual:** 2.0.0  
**Mantenedor:** NNews Team
