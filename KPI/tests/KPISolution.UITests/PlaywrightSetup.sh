#!/bin/bash

echo "Setting up Playwright for .NET"
echo "============================"

# Step 1: Make sure we have the latest Playwright packages
echo "Step 1: Restoring the project to ensure latest Playwright packages..."
dotnet restore

# Step 2: Build the project to ensure Playwright binaries are copied
echo "Step 2: Building the project..."
dotnet build

# Step 3: Try to find the playwright-driver directly
echo "Step 3: Finding and using playwright-driver..."
export PLAYWRIGHT_BROWSERS_PATH=0
export PLAYWRIGHT_SKIP_BROWSER_DOWNLOAD=0
export PLAYWRIGHT_SKIP_VALIDATE_HOST_REQUIREMENTS=1

# First check the output directory
DRIVER_DIR=$(find ./bin -name "playwright-driver" | head -1)

if [ -z "$DRIVER_DIR" ]; then
    # Check the NuGet packages folder
    DRIVER_DIR=$(find ~/.nuget/packages -name "playwright-driver" | head -1)
fi

if [ -n "$DRIVER_DIR" ]; then
    echo "Found Playwright driver at: $DRIVER_DIR"
    chmod +x "$DRIVER_DIR"
    echo "Installing browsers using playwright-driver..."
    "$DRIVER_DIR" install
    echo "Browser installation completed!"
else
    echo "Could not find playwright-driver."
    echo "Looking for alternatives..."
    
    # Check for playwright CLI directly
    CLI_DIR=$(find . -name "playwright.dll" | grep -v "runtimes" | head -1)
    if [ -n "$CLI_DIR" ]; then
        echo "Found Playwright CLI at: $CLI_DIR"
        echo "Installing browsers using dotnet exec..."
        dotnet exec "$CLI_DIR" install
        echo "Browser installation completed!"
    else
        echo "Could not find Playwright CLI. Using fallback method..."
        # Try using the Microsoft.Playwright assembly directly
        CLI_DIR=$(find ~/.nuget/packages -name "Microsoft.Playwright.dll" | head -1)
        if [ -n "$CLI_DIR" ]; then
            echo "Found Microsoft.Playwright.dll at: $CLI_DIR"
            echo "Installing browsers using direct assembly..."
            dotnet exec "$CLI_DIR" install
            echo "Browser installation completed!"
        else
            echo "ERROR: Could not find any Playwright installation methods."
            echo "Please try running: dotnet add package Microsoft.Playwright.NUnit"
            exit 1
        fi
    fi
fi

echo "Step 4: Verifying installation..."
dotnet run -- check

echo "Playwright setup complete!" 