//=========================================================================================================
//Note: Data Managing.
//Date Created: 2012/04/17 by 风宇冲
//Date Modified: 2012/12/14 by 风宇冲
//=========================================================================================================
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.Security.Cryptography;


/// <summary>
/// 储存数据的类
/// </summary>
public class GameData
{
    /// <summary>
    /// 密钥,用于防止拷贝存档
    /// </summary>
    public string key;

    public string background;

    public int pieceCount;
    public string pieceImage;
    public string pieceShape;
    public string pieceStyle;


    public List<string> records = new List<string>();

    public GameData()
    {
        background = "bg1";

        pieceCount = 24;
        pieceImage = "img";
        pieceShape = "normal";
        pieceStyle = "none";
    }
}

/// <summary>
/// 管理数据储存的类
/// </summary>
public class GameDataManager : MonoBehaviour
{
    /// <summary>
    /// 存档文件的名称
    /// </summary>
    private string dataFileName = "puzzle.dat";

    /// <summary>
    /// xml 存储 对象
    /// </summary>
    private XmlSaver xs = new XmlSaver();

    /// <summary>
    /// 游戏数据
    /// </summary>
    public GameData gameData;

    public void Awake()
    {
        gameData = new GameData();

        // 设定密钥，根据具体平台设定
        gameData.key = SystemInfo.deviceUniqueIdentifier;

        // 加载数据
        Load();
    }

    //存档时调用的函数//
    public void Save()
    {
        string gameDataFile = GetDataPath() + "/" + dataFileName;
        string dataString = xs.SerializeObject(gameData, typeof(GameData));
        xs.CreateXML(gameDataFile, dataString);
    }

    /// <summary>
    /// 读档时调用的函数
    /// </summary>
    public void Load()
    {
        string gameDataFile = GetDataPath() + "/" + dataFileName;
        if (File.Exists(gameDataFile))
        {
            string dataString = xs.LoadXML(gameDataFile);
            GameData gameDataFromXML = xs.DeserializeObject(dataString, typeof(GameData)) as GameData;

            //是合法存档//
            if (gameDataFromXML.key == gameData.key)
            {
                gameData = gameDataFromXML;
            }
            //是非法拷贝存档//
            else
            {
                //留空：游戏启动后数据清零，存档后作弊档被自动覆盖//
            }
        }
        else
        {
            if (gameData != null)
                Save();
        }
    }

    /// <summary>
    /// 获取路径
    /// </summary>
    /// <returns>路径</returns>
    private static string GetDataPath()
    {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents
        // Application.dataPath returns ar/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data             
        // Strip "/Data" from path
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);


            // Strip application name
            path = path.Substring(0, path.LastIndexOf('/'));
            return path + "/Documents";
        }
        else
            //    return Application.dataPath + "/Resources";
            return Application.dataPath;
    }
}