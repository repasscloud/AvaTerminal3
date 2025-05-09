#!/usr/bin/env zsh

killall -HUP AvaTerminal3 2>/dev/null
#sort -u ./GlobalUsings.cs -o ./GlobalUsings.cs
dotnet clean

dotnet run -f net9.0-maccatalyst 2>&1 | tee _logs/launch.log
