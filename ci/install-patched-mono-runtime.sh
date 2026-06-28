#!/usr/bin/env bash

set -euxo pipefail

rm "C:\\Users\\runneradmin\\.nuget\\packages\\embeddinator-4000\\0.4.0\\external\\Xamarin.Android\\lib\\xbuild\\Xamarin\\Android\\lib\\arm64-v8a\\libmonosgen-2.0.so" || true

cp ./ci/libmonosgen-2.0-assert-patch.so "C:\\Users\\runneradmin\\.nuget\\packages\\embeddinator-4000\\0.4.0\\external\\Xamarin.Android\\lib\\xbuild\\Xamarin\\Android\\lib\\arm64-v8a\\libmonosgen-2.0.so" || true
