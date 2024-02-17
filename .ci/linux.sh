#!/bin/sh

# This script expects to be running on Ubuntu 20.04

# Prevent hangs when installing tzdata (some dependency down the line)
export DEBIAN_FRONTEND=noninteractive
export TZ=Etc/UTC

# Install some base tools
apt-get install -y wget lsb-release software-properties-common gpg cmake ninja-build

# Install clang 17
wget https://apt.llvm.org/llvm.sh
chmod +x llvm.sh
./llvm.sh 17

if [ $TARGET_RID = "linux-x64" ]; then
	# Nothing special needed here
	export EXTRA_CMAKE_ARGS=""
	# Install SDL2 dependencies
	apt-get install -y libasound2-dev libpulse-dev libaudio-dev libjack-jackd2-dev libsamplerate0-dev \
		libx11-dev libxext-dev libxrandr-dev libxcursor-dev libxfixes-dev libxi-dev \
		libxss-dev libwayland-dev libxkbcommon-dev libdrm-dev libgbm-dev libgl1-mesa-dev \
		libgles2-mesa-dev libegl1-mesa-dev libdbus-1-dev libibus-1.0-dev fcitx-libs-dev \
		libudev-dev libusb-1.0-0-dev pkg-config
	# Install .NET AOT dependencies
	apt-get install -y zlib1g-dev
elif [ $TARGET_RID = "linux-arm64" ]; then
	# Install aarch64 cross compiling setup
	apt-get install -y gcc-aarch64-linux-gnu g++-aarch64-linux-gnu pkg-config-aarch64-linux-gnu dpkg-dev
	export PKG_CONFIG=aarch64-linux-gnu-pkg-config
	export EXTRA_CMAKE_ARGS="-DCMAKE_SYSTEM_NAME=Linux -DCMAKE_SYSTEM_PROCESSOR=aarch64 -DCMAKE_C_FLAGS=--target=aarch64-linux-gnu -DCMAKE_CXX_FLAGS=--target=aarch64-linux-gnu"
	# Enable ARM64 packages
	echo "deb [arch=arm64] http://ports.ubuntu.com/ubuntu-ports/ focal main restricted universe" | tee /etc/apt/sources.list.d/arm64.list
	echo "deb [arch=arm64] http://ports.ubuntu.com/ubuntu-ports/ focal-updates main restricted universe" | tee -a /etc/apt/sources.list.d/arm64.list
	sed -i -e 's/deb http/deb [arch=amd64] http/g' /etc/apt/sources.list
	dpkg --add-architecture arm64
	apt-get update
	# Install SDL2 dependencies
	apt-get install -y libasound2-dev:arm64 libpulse-dev:arm64 libaudio-dev:arm64 libjack-jackd2-dev:arm64 libsamplerate0-dev:arm64 \
		libx11-dev:arm64 libxext-dev:arm64 libxrandr-dev:arm64 libxcursor-dev:arm64 libxfixes-dev:arm64 libxi-dev:arm64 \
		libxss-dev:arm64 libwayland-dev:arm64 libxkbcommon-dev:arm64 libdrm-dev:arm64 libgbm-dev:arm64 libgl1-mesa-dev:arm64 \
		libgles2-mesa-dev:arm64 libegl1-mesa-dev:arm64 libdbus-1-dev:arm64 libibus-1.0-dev:arm64 fcitx-libs-dev:arm64 \
		libudev-dev:arm64 libusb-1.0-0-dev:arm64
	# Install .NET AOT dependencies
	apt-get install -y zlib1g-dev:arm64
else
	echo "TARGET_RID must be linux-x64 or linux-arm64 (got $TARGET_RID)"
	exit 1
fi

CMakeNinjaBuild() {
	mkdir build_$1_static_$TARGET_RID
	cd build_$1_static_$TARGET_RID
	cmake ../../externals/$1 \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_C_COMPILER=clang-17 \
		-DCMAKE_CXX_COMPILER=clang++-17 \
		$EXTRA_CMAKE_ARGS \
		-G Ninja \
		-DGSR_SHARED=OFF
	ninja
	cd ..
}

CMakeNinjaBuild cimgui
CMakeNinjaBuild SDL2
CMakeNinjaBuild gambatte
CMakeNinjaBuild mgba

# Install dotnet8 sdk
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Build GSR
cd ..
$HOME/.dotnet/dotnet publish ./GSR/GSR.csproj -r $TARGET_RID -p:CppCompilerAndLinker=clang-17 -p:LinkerFlavor=lld-17 -p:ObjCopyName=llvm-objcopy-17