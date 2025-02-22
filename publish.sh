#!/bin/bash

PROJECT_PATH="./src/TaskTracker.CLI/TaskTracker.CLI.csproj"
OUTPUT_BASE_PATH="./publish"
RUNTIMES=("win-x64" "linux-x64" "osx-x64" "osx-arm64")

for RUNTIME in "${RUNTIMES[@]}"; do
    OUTPUT_PATH="$OUTPUT_BASE_PATH/$RUNTIME"
    echo "Publishing .NET application for $RUNTIME..."
    dotnet publish "$PROJECT_PATH" -c Release -r "$RUNTIME" -o "$OUTPUT_PATH" \
        -p:PublishSingleFile=true \
        -p:IncludeNativeLibraries=true \
        -p:PublishReadyToRun=true \
        -p:DebugType=embedded \
        --self-contained=true
    echo "Publish completed for $RUNTIME! Output directory: $OUTPUT_PATH"
done

APP_NAME="ttracker"
VERSION="1.0.0"
ARCH="amd64"
BUILD_DIR="./publish/linux-x64/deb-build"
OUTPUT_DIR="./publish/linux-x64/deb"
INSTALL_DIR="/usr/local/bin"
EXECUTABLE="ttracker"
rm -rf "$BUILD_DIR" "$OUTPUT_DIR"
mkdir -p "$BUILD_DIR/DEBIAN" "$BUILD_DIR$INSTALL_DIR" "$OUTPUT_DIR"
cat <<EOF > "$BUILD_DIR/DEBIAN/control"
Package: $APP_NAME
Version: $VERSION
Architecture: $ARCH
Maintainer: Your Name <your.email@example.com>
Description: Task Tracker CLI Application
EOF
cp "./publish/linux-x64/$EXECUTABLE" "$BUILD_DIR$INSTALL_DIR/$APP_NAME"
chmod +x "$BUILD_DIR$INSTALL_DIR/$APP_NAME"
fakeroot dpkg-deb --build "$BUILD_DIR" "$OUTPUT_DIR/${APP_NAME}_${VERSION}_${ARCH}.deb"
echo "DEB package created: $OUTPUT_DIR/${APP_NAME}_${VERSION}_${ARCH}.deb"


AUTOCOMPLETE_DIR="/etc/bash_completion.d"
AUTOCOMPLETE_FILE="$AUTOCOMPLETE_DIR/$APP_NAME"
mkdir -p "$AUTOCOMPLETE_DIR"
cat <<EOF > "$AUTOCOMPLETE_FILE"
#!/bin/bash

_${APP_NAME}_completion() {
    local cur prev opts
    COMPREPLY=()
    cur="${COMP_WORDS[COMP_CWORD]}"
    opts="list add update delete mark -h --help"

    COMPREPLY=( $(compgen -W "$opts" -- "$cur") )
    return 0
}

complete -F _${APP_NAME}_completion $APP_NAME
EOF
chmod +x "$AUTOCOMPLETE_FILE"
echo "Bash autocomplete script installed at $AUTOCOMPLETE_FILE. Restart your shell or run 'source $AUTOCOMPLETE_FILE' to enable it."
