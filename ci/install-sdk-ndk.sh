#!/usr/bin/env bash

set -euxo pipefail

# Embeddinator uses Xamarin Android Tools to find Android SDKs, NDKs, and Java SDKs
# The main registry key is decided by the envvar read here under Visual Studio and MSBuild
# https://github.com/xamarin/xamarin-android-tools/blob/294f4471a76da6df798c8520a8f8da0e6e83d3a5/src/Xamarin.Android.Tools.AndroidSdk/Sdks/AndroidSdkWindows.cs#L65
# that is set by Xamarin Android here in the build tasks
# https://github.com/xamarin/xamarin-android/blob/d4c8f077faefdfc6174355848a9d8c74ecaa8f56/src/Xamarin.Android.Build.Tasks/Tasks/SetVsMonoAndroidRegistryKey.cs
# and ultimately obtained by reading out the Visual Studio installation id, which seems to be unique
# https://github.com/xamarin/xamarin-android/blob/a677c1794db64d5559f53a960927447bac3063a2/src/Xamarin.Android.Build.Tasks/MSBuild/Xamarin/Xamarin.Android.Sdk.props#L16


cat <<EOF > ~/.config/xbuild/monodroid-config.xml
<?xml version="1.0" encoding="utf-8"?>
<monodroid>
  <android-sdk path="$ANDROID_SDK_ROOT" />
  <java-sdk path="$JAVA_HOME_11_X64" />
  <android-ndk path="$ANDROID_SDK_ROOT/ndk/android-ndk-r15c" />
</monodroid>
EOF

# Manually install Android SDK Platform 24, the most recent version that still works with Embeddinator 0.4.0
# Manually install Android SDK Platform 25, version needed by Xamarin
(yes || true) | ${ANDROID_SDK_ROOT}/cmdline-tools/latest/bin/sdkmanager.bat --sdk_root=$ANDROID_SDK_ROOT "platform-tools" "build-tools;30.0.3" "platforms;android-24" "platforms;android-25" "cmdline-tools:5.0"

# Manually install Android NDK r15c, the most recent version that still works with Embeddinator 0.4.0
curl --retry 5 --connect-timeout 30 --location --remote-header-name --remote-name https://dl.google.com/android/repository/android-ndk-r15c-linux-x86_64.zip
echo "0bf02d4e8b85fd770fd7b9b2cdec57f9441f27a2 *android-ndk-r15c-linux-x86_64.zip" | sha1sum -c
7z x android-ndk-r15c-linux-x86_64.zip -o '$ANDROID_SDK_ROOT/ndk'
