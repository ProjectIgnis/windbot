#!/usr/bin/env bash

set -euxo pipefail

rm "~/.nuget/packages/embeddinator-4000/0.4.0/external/Xamarin.Android/lib/xbuild/Xamarin/Android/lib/arm64-v8a/libmonosgen-2.0.so"

cp ./ci/libmonosgen-2.0-assert-patch.so "~/.nuget/packages/embeddinator-4000/0.4.0/external/Xamarin.Android/lib/xbuild/Xamarin/Android/lib/arm64-v8a/libmonosgen-2.0.so"
