DOTNET=dotnet
ARCH=win-x64
WINE=wine

all:
	$(DOTNET) build --no-restore

build-arch:
	$(DOTNET) publish --self-contained true --runtime $(ARCH) 

wine:
	$(DOTNET) publish --self-contained true --runtime $(ARCH) && $(WINE) ./bin/Debug/net5.0/win-x64/publish/SAL.exe