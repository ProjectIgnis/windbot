#!/usr/bin/env bash

set -euxo pipefail

# Manually install Android NDK r15c, the most recent version that still works with Embeddinator 0.4.0
curl --retry 5 --connect-timeout 30 --location --remote-header-name --remote-name https://dl.google.com/android/repository/android-ndk-r15c-linux-x86_64.zip
echo "0bf02d4e8b85fd770fd7b9b2cdec57f9441f27a2 *android-ndk-r15c-linux-x86_64.zip" | sha1sum -c
mkdir -p "$ANDROID_SDK_ROOT/ndk"
unzip android-ndk-r15c-linux-x86_64.zip -d "$ANDROID_SDK_ROOT/ndk"

# Embeddinator loads the paths to find to find Android SDKs, NDKs, and Java SDKs from ~/.config/xbuild/monodroid-config.xml
mkdir -p ~/.config/xbuild
cat <<EOF > ~/.config/xbuild/monodroid-config.xml
<?xml version="1.0" encoding="utf-8"?>
<monodroid>
  <android-sdk path="$ANDROID_SDK_ROOT" />
  <java-sdk path="$JAVA_HOME_11_X64" />
  <android-ndk path="$ANDROID_SDK_ROOT/ndk/android-ndk-r15c" />
</monodroid>
EOF

# Manually install Android SDK Platform 24, the most recent version that still works with Embeddinator 0.4.0
# Manually install Android SDK Platform 25, build tools 30.0.3 and cmdline-tools 5.0, required by Xamarin
(yes || true) | ${ANDROID_SDK_ROOT}/cmdline-tools/latest/bin/sdkmanager --sdk_root=$ANDROID_SDK_ROOT "platform-tools" "build-tools;30.0.3" "platforms;android-24" "platforms;android-25" "cmdline-tools;5.0"

