# AppLinker4Unity

**AppLinker4Unity** is a Unity Android plugin (AAR) that allows you to fetch list of installed applications on a device, get app icons, names and perform actions such as opening the app or navigating to its settings page — that you can use on your Unity UI.

I've use this on one of my repo [Simple Launcher](https://github.com/ACEKaito1412/SimpleLauncher).

---

## 📦 Features

- Fetch list of all installed launcher apps.
- Get app names, package names, and app icons (Base64).
- Open app directly or go to app settings.
- Unity wrapper included.

---

## 🛠️ AAR Setup Requirements

### 1. **Android Manifest Changes**

Make sure your `AndroidManifest.xml` includes the following under your `<manifest>` tag to allow querying for installed apps. Check the `manifest.xml` file in the repository.

## 🔧 How to use the Methods in Plugin 

For usage and example of wrapper for the plugin, check the `PluginWrapper.cs` file in the repository.

### Prepare Unity Activity and Library Class

```cs
private AndroidJavaClass unityClass;
private AndroidJavaObject javaClass;
private AndroidJavaObject unityActivity;

void Start(){

    unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    javaClass = new AndroidJavaObject("com.acekaito.applinker4unity.PluginInstance");
    unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
}
```

### Method

**recieveUnityActivity(Activity unityActivity)** :
Must be called first. Passes Unity's current activity to the plugin.

```cs
javaClass.CallStatic("recieveUnityActivity", unityActivity);
```
**GetAppNames()** : Returns a String[] list of all installed app names.

```cs
string[] appList = javaClass.CallStatic<string[]>("GetAppNames");

```

**GetAppIcon()** : Returns a String[] list of app icons as Base64 PNGs.

```cs
string[] appIconList = javaClass.CallStatic<string[]>("GetAppIcon");
```

**GetAppPackageNames()** : Returns a String[] list of app package names.

```cs
string[] appPackageList = javaClass.CallStatic<string[]>("GetAppPackageName");
```

**OpenApp(String packageName)** : Launches the app using the provided package name.

```cs
javaClass.CallStatic("OpenApp", packageName);
```

**OpenAppInfo(String packageName)** : Opens the app's settings/info page.
```cs
javaClass.CallStatic("OpenAppInfo", packageName);
```

## Latest Build

- [Download Latest AAR](aar-build/applinker4unity-latest.aar)