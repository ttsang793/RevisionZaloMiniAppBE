#!/bin/bash
export ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
dotnet run