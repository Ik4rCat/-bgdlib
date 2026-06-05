PROJ := bgdlib/bgdlib.csproj
FW   := net10.0-android

.PHONY: build run release clean

build:
	dotnet build $(PROJ) -f $(FW)

run:
	dotnet run --project $(PROJ) -f $(FW)

release:
	dotnet build $(PROJ) -f $(FW) -c Release

clean:
	dotnet clean $(PROJ)