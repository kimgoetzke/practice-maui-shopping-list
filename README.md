# The Shopping List Project

This repository contains a simple, minimalist shopping list Android app written in C# using .NET 8 MAUI, the
CommunityToolkit, and SQLite. The project uses Appium and NUnit for UI testing. It was my first attempt to create an
app and use .NET MAUI. The goal was to learn about .NET MAUI cross-platform development, Android app development, and
to build something semi-useful.

See the [kimgoetzke/practice-maui-listem](https://github.com/kimgoetzke/practice-maui-listem) for a revamped design 
and conversion to a to-do list app, allowing for multiple lists and a little more configuration.

![Demo GIF](./assets/demo.gif)

## Features

* An app containing a minimalist shopping list
* The list indicates quantity, where to buy an item, and whether or not an item is important
* Store names can be configured (optional)
* A list can be exported to the clipboard as text
* A list can be imported from a comma-separated string from the clipboard and merged with the current list
* All data is stored in a SQLite database on the device
* Native confirmation prompts are used for destructive actions
* A simple welcome popup is shown on first launch
* A dark theme is available
* Icons used are CC0 from [iconsDB.com](https://www.iconsdb.com/)

## How to build develop

1. Set environment variables for builds and running tests
    1. `ANDROID_HOME` - the absolute path of the Android SDK
    2. `SHOPPING_LIST_DEBUG_APK` - the absolute path of the debug APK
    3. `SHOPPING_LIST_RELEASE_APK` - the absolute path of the release APK

## How to build the APK

Create APK with:

```shell
dotnet publish -f:net8.0-android -c:Release /p:AndroidSdkDirectory=$env:ANDROID_HOME
```

This assumes that the Android SDK is installed and the `ANDROID_HOME` environment variable is set.

APK file can then be found in `ShoppingList\bin\Release\net8.0-android\publish\` and installed directly on any Android
phone.

## How to run tests

_Note: Currently, I am unable to get Appium to install the APK correctly on the emulator. The only way to make the app
start during tests is to first install the APK on the device, close the welcome popup, and then run the tests. If the
APK is ever installed by Appium, the device needs to be wiped and the APK installed without Appium for the tests to
pass again._

To run the tests:
1. Install the APK on the device/emulator
2. Launch the app to close the welcome popup
3. Close it again
4. Run the tests via your IDE or `donet test`

Now you can keep running the tests without reinstalling the APK (until you make changes to the code).

Alternatively, you could also run test on a device/emulator while the Appium server is running. For this you would have
to change the `AppiumSetup` accordingly and then:

1. Start an Android emulator or connect an Android device
2. Run Appium server with `appium`
3. Run the tests via your IDE or `donet test`
