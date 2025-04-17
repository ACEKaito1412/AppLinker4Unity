package com.acekaito.applinker4unity;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.drawable.AdaptiveIconDrawable;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.os.Build;
import android.util.Base64;
import android.util.Log;
import java.io.ByteArrayOutputStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;


public class PluginInstance {
    private static final String TAG = "Unity-PluginInstance";

    private static Activity _unityActivity;

    private static List<MyAppInfo> appList;

    private static void recieveUnityActivity(Activity unityActivity) {
        _unityActivity = unityActivity;

        if (_unityActivity != null) {
            Log.d(TAG, "recieveUnityActivity: Activity is Received");
            getAppData();
        }else{
            Log.d(TAG, "recieveUnityActivity: Activity is not Received");
        }
    }

    private static void getAppData() {
        appList = new ArrayList<>();

        PackageManager pm = _unityActivity.getPackageManager();

        Intent i = new Intent(Intent.ACTION_MAIN, null);
        i.addCategory(Intent.CATEGORY_LAUNCHER);


        List<ResolveInfo> allApps = pm.queryIntentActivities(i, 0);

        Collections.sort(allApps, new Comparator<ResolveInfo>() {
            @Override
            public int compare(ResolveInfo a, ResolveInfo b) {
                CharSequence labelA = a.loadLabel(pm);
                CharSequence labelB = b.loadLabel(pm);
                return labelA.toString().compareToIgnoreCase(labelB.toString());
            }
        });

        for(ResolveInfo ri:allApps) {
            MyAppInfo appInfo = new MyAppInfo();
            appInfo.appName = ri.loadLabel(pm).toString();
            appInfo.appPackageName = ri.activityInfo.packageName;

            Drawable icon = ri.activityInfo.loadIcon(pm).getCurrent(); // use getCurrent() if needed
            if (icon instanceof BitmapDrawable) {
                appInfo.appIcon = icon;
                appInfo.appIconBitmap = ((BitmapDrawable) icon).getBitmap();
            } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O && icon instanceof AdaptiveIconDrawable) {
                appInfo.appIcon = icon;
                appInfo.appIconBitmap = getBitmapFromDrawable(icon);
            } else {
                // Handle other drawable types if necessary
                appInfo.appIcon = icon;
                appInfo.appIconBitmap = getBitmapFromDrawable(icon);
            }


            appList.add(appInfo);
        }

        Log.d(TAG, "GetAppData: " + allApps.size());
    }

    private static Bitmap getBitmapFromDrawable(Drawable drawable) {
        Bitmap bitmap = Bitmap.createBitmap(drawable.getIntrinsicWidth(), drawable.getIntrinsicHeight(), Bitmap.Config.ARGB_8888);
        Canvas canvas = new Canvas(bitmap);
        drawable.setBounds(0, 0, canvas.getWidth(), canvas.getHeight());
        drawable.draw(canvas);
        return bitmap;
    }

    public String TestHello(){
        return "Connection is ready.";
    }

    public int TestAdd(int i, int j){
        return i + j;
    }

    public static String[] GetAppNames() {
        List<String> appNameList = new ArrayList<>();
        for (MyAppInfo appInfo : appList) {
            appNameList.add(appInfo.appName);
        }

        return appNameList.toArray(new String[0]);
    }

    public static String[] GetAppIcon() {
        List<String> AppIcons = new ArrayList<>();

        for (MyAppInfo appInfo : appList) {
            try {
                // Convert bitmap to base64 string
                Bitmap bitmap = appInfo.appIconBitmap;
                if (bitmap != null) {
                    ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
                    bitmap.compress(Bitmap.CompressFormat.PNG, 100, byteArrayOutputStream);
                    byte[] byteArray = byteArrayOutputStream.toByteArray();
                    AppIcons.add(Base64.encodeToString(byteArray, Base64.DEFAULT));
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        Log.d(TAG, "GetAppIcon: Returning List is A Success " + AppIcons.size());
        return AppIcons.toArray(new String[0]);
    }

    public static String[] GetAppPackageNames(){
        List<String> AppPackageName = new ArrayList<>();
        for (MyAppInfo appInfo: appList) {
            AppPackageName.add(appInfo.appPackageName);
        }

        return AppPackageName.toArray(new String[0]);
    }

    public static void OpenApp(String packageName){
        Intent intent = _unityActivity.getPackageManager().getLaunchIntentForPackage(packageName);

        _unityActivity.startActivity(intent);

        Log.d(TAG, "OpenApp: " + packageName);
    }

    public static void OpenAppInfo(String packageName){
        Intent intent = new Intent(android.provider.Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
        intent.addCategory(Intent.CATEGORY_DEFAULT);
        intent.setData(Uri.parse("package:" + packageName));
        _unityActivity.startActivity(intent);

        Log.d(TAG, "OpenAppInfo: " + packageName + " Result: OK");
    }
}