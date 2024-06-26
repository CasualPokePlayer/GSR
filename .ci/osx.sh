#!/bin/sh

# Install build tools
brew install ninja create-dmg

CMakeNinjaBuild() {
	# One time for x64
	mkdir build_$1_static_osx-x64
	cd build_$1_static_osx-x64
	cmake ../../externals/$1 \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_C_COMPILER=clang \
		-DCMAKE_CXX_COMPILER=clang++ \
		-DCMAKE_OBJC_COMPILER=clang \
		-DCMAKE_OBJCXX_COMPILER=clang++ \
		-DCMAKE_OSX_ARCHITECTURES=x86_64 \
		-DCMAKE_SYSTEM_NAME=Darwin \
		-DCMAKE_SYSTEM_PROCESSOR=x86_64 \
		-G Ninja \
		-DGSR_SHARED=OFF
	ninja
	cd ..
	# Another time for arm64
	mkdir build_$1_static_osx-arm64
	cd build_$1_static_osx-arm64
	cmake ../../externals/$1 \
		-DCMAKE_BUILD_TYPE=Release \
		-DCMAKE_C_COMPILER=clang \
		-DCMAKE_CXX_COMPILER=clang++ \
		-DCMAKE_OBJC_COMPILER=clang \
		-DCMAKE_OBJCXX_COMPILER=clang++ \
		-DCMAKE_OSX_ARCHITECTURES=arm64 \
		-DCMAKE_SYSTEM_NAME=Darwin \
		-DCMAKE_SYSTEM_PROCESSOR=arm64 \
		-G Ninja \
		-DGSR_SHARED=OFF
	ninja
	cd ..
}

CMakeNinjaBuild cimgui
CMakeNinjaBuild SDL2
CMakeNinjaBuild gambatte
CMakeNinjaBuild mgba
CMakeNinjaBuild export_helper

# Build GSR
cd ..
dotnet workload install macos
dotnet publish -r osx-x64
dotnet publish -r osx-arm64

# Abort if the build failed for whatever reason
if [ ! -f output/osx-x64/GSR.app/Contents/MacOS/GSR ] || [ ! -f output/osx-arm64//GSR.app/Contents/MacOS/GSR ]; then
	echo "dotnet publish failed, aborting"
	exit 1
fi

# Merge the binaries together
mkdir output/$TARGET_RID
cp -a output/osx-x64/GSR.app output/$TARGET_RID
lipo output/osx-x64/GSR.app/Contents/MacOS/GSR output/osx-arm64/GSR.app/Contents/MacOS/GSR -create -output output/$TARGET_RID/GSR.app/Contents/MacOS/GSR

# Also have to do this with all the dylibs bundled by dotnet
cd output/osx-x64
for dylib in GSR.app/Contents/MonoBundle/libSystem.*.dylib; do
	lipo $dylib ../osx-arm64/$dylib -create -output ../$TARGET_RID/$dylib
done
cd ../..

# Resign the binary
codesign -s - --deep --force output/$TARGET_RID/GSR.app

# Output a dmg
mkdir output/$TARGET_RID/publish
create-dmg output/$TARGET_RID/publish/GSR.dmg output/$TARGET_RID/GSR.app
