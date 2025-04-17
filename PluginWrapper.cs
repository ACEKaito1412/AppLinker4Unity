using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Linq;


public class PluginWrapper : MonoBehaviour
{
    public GameObject appBase;
    public TextMeshProUGUI text;
    public RectTransform CanvasUI;
    public GridLayoutGroup Container;

    private AndroidJavaClass unityClass;
    private AndroidJavaObject javaClass;
    private AndroidJavaObject unityActivity;

    private List<string> appNames;
    private List<string> appIcons;
    private List<string> appPackages;

    private int appSize = 60;
    

    // Start is called before the first frame update
    void Start()
    {   
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        javaClass = new AndroidJavaObject("com.acekaito.plugin.PluginInstance");

        try
        {
            unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }catch (Exception ex){
            Debug.Log("ST : ACT");
            appNames = new List<string>();
        }

        //javaClass.Call("Hello");
        //javaClass.Call("Add", 5, 10);

        //text.text = javaClass.Call<int>("Subtract", 10).ToString();

        if(javaClass == null || unityActivity == null)
        {
            text.text = "ST : NPP";
        }else{
            
            javaClass.CallStatic("recieveUnityActivity", unityActivity);

            // Call the Java method to get the app names
            string[] appList = javaClass.CallStatic<string[]>("GetAppNames");
            string[] appIconList = javaClass.CallStatic<string[]>("GetAppIcon");
            string[] appPackageList = javaClass.CallStatic<string[]>("GetAppPackageName");


            // Convert the string array to a List<string>
            appNames = new List<string>(appList);
            appIcons = new List<string>(appIconList);
            appPackages = new List<string>(appPackageList);


            var combined = appNames
                .Zip(appIcons, (name, icon) => new { Name = name, Icon = icon })
                .Zip(appPackages, (ni, package) => new { Name = ni.Name, Icon = ni.Icon, Package = package })
                .OrderBy(item => item.Name)  // Sort by appNames (Name property)
                .ToList();

            appNames = combined.Select(item => item.Name).ToList();
            appIcons = combined.Select(item => item.Icon).ToList();
            appPackages = combined.Select(item => item.Package).ToList();


            appSize = appList.Length;

            text.text = "ST : OK";
        }

        CreateAppList();
    }


    public class StringListWrapper
    {
        public List<string> items;
    }

    public void CreateAppList()
    {
        int currAppPos = 0;
        for (int x = 0; x < appSize; x++)
        {
            Debug.Log("hello.");

            GameObject gameObject = Instantiate(appBase, new Vector3(0,0,0), Quaternion.identity);
            gameObject.transform.parent = Container.transform;
            gameObject.SetActive(true);

            Image image = gameObject.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI tm = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            LongPressClick longpress = gameObject.GetComponent<LongPressClick>();


            // this will raname out gameobject to the packagename
            if(appPackages != null)
            {
                gameObject.name = appPackages[currAppPos];
            }
            else
            {
                gameObject.name = "App " + x;
                tm.text = "App " + x;
            }


            // this will set the name and the sprite from out base64 string list
            if (appIcons != null && appNames != null && image != null && tm != null && currAppPos < appSize)
            {
                Debug.Log("Wow");
                Sprite newSprite = CreateSpriteFromBase64(appIcons[currAppPos]);

                image.sprite = newSprite;
                String strName = appNames[currAppPos];

                String app_name = "";
                for(int i = 0; i < strName.Length; i++){
                    if(i < 12){
                        app_name += strName[i];
                    }else{
                        app_name += "..";
                        break;
                    }
                }

                tm.text = app_name;

                currAppPos++;
            }

            // this will set the listiner for our click event on each button
            Button btn = gameObject.GetComponent<Button>();

            if (btn != null)
            {
                btn.onClick.AddListener(() => OnImageClick(gameObject, longpress));
            }
            else
            {
                Debug.LogError("We cant identify the button here");
            }
            

            if(longpress != null) {
                longpress.OnLongPress += () => OnLongPress(gameObject);
            }else{
                Debug.Log("cant find the long press");
            }
        }
    }

    private void OnImageClick(GameObject gameObject, LongPressClick longpress)
    {
        Debug.Log(gameObject.name);
        if(appPackages != null && !longpress.longPressTriggered)
        {
            javaClass.CallStatic("OpenApp", gameObject.name);
        }
    }

    private void OnLongPress(GameObject gameObject){
        if(appPackages != null){
            javaClass.CallStatic("OpenAppInfo", gameObject.name);
        }else{
            Debug.Log("Long press triggered");
        }
    }

    Sprite CreateSpriteFromBase64(string base64String)
    {
        try
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            Texture2D texture = new Texture2D(2, 2); // Create a temporary texture
            if (texture.LoadImage(imageBytes))
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image from byte array.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error converting Base64 string to sprite: " + e.Message);
            return null;
        }
    }
}