DOTNET=dotnet
ARCH=win-x64
WINE=wine
BINROOT=bin/Debug/net5.0/win-x64
BINROOT_RELEASE=bin/Release/net5.0/win-x64
PUBLISH_BIN_DIR=SAL
VERSION=v0.1.2

restore:
	$(DOTNET) tool restore
	$(DOTNET) paket install

all:
	$(DOTNET) build --no-restore

build-arch:
	$(DOTNET) publish --self-contained true --runtime $(ARCH)

wine:
	$(DOTNET) publish --self-contained true --runtime $(ARCH) && $(WINE) ./$(BINROOT)/publish/SAL.exe

bundle:
	rm -rf ./$(BINROOT_RELEASE)
	$(DOTNET) publish --self-contained true --runtime $(ARCH) -c Release -p:PublishTrimmed=true && cd ./$(BINROOT_RELEASE) && cp -r ./publish $(PUBLISH_BIN_DIR) && zip -9 -r SAL-$(ARCH)-$(VERSION).zip ./$(PUBLISH_BIN_DIR)