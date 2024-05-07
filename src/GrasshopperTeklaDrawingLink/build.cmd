rmdir .\bin /s /q

msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2019 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2020 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2021 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2022 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2023 /t:Rebuild
msbuild GrasshopperTeklaDrawingLink.csproj /t:restore -p:Configuration=_2024 /t:Rebuild

for /R .\bin %%f in (*.gha) do copy %%f .\bin\

echo