#!/usr/bin/env zsh

dotnet publish ./AvaTerminal3.csproj \
  -f net9.0-maccatalyst \
  -c Release \
  -r maccatalyst-arm64 \
  -p:MtouchLink=SdkOnly \
  -p:CreatePackage=false \
  -p:EnableCodeSigning=false \
  -p:EnablePackageSigning=false \
  -p:UseHardenedRuntime=true