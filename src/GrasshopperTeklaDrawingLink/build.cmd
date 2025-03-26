rmdir .\bin /s /q

msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2019 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2019i /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2020 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2021 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2022 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2023 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2024 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=2025 /t:Rebuild

for /R .\bin %%f in (*.gha) do copy %%f .\bin\

echo