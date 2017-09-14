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

    /// <summary>
    /// 背景名
    /// </summary>
    public string background;

    /// <summary>
    /// 拼图个数
    /// </summary>
    public int pieceCount;

    /// <summary>
    /// 拼图图像
    /// </summary>
    public string pieceImage;

    /// <summary>
    /// 拼图形状
    /// </summary>
    public string pieceShape;

    /// <summary>
    /// 拼图风格
    /// </summary>
    public string pieceStyle;

    /// <summary>
    /// 是否显示原图
    /// </summary>
    public bool showImage;

    /// <summary>
    /// 是否旋转拼图
    /// </summary>
    public bool rotatePuzzle;

    /// <summary>
    /// 游戏纪录
    /// </summary>
    public List<string> records = new List<string>();


    /// <summary>
    /// 构造函数，设置默认值
    /// </summary>
    public GameData()
    {
        background = "bg1";

        pieceCount = 24;
        pieceImage = "img";
        pieceShape = "normal";
        pieceStyle = "none";

        showImage = false;
        rotatePuzzle = false;
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

    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        // 新建游戏数数据
        gameData = new GameData();

        // 设定密钥，根据具体平台设定
        gameData.key = SystemInfo.deviceUniqueIdentifier;

        // 加载数据
        Load();
    }

    /// <summary>
    /// 卸载时保存数据
    /// </summary>
    void OnDestroy()
    {
        Save();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    public void Save()
    {
        // 获取文件名
        string gameDataFile = GetDataPath() + "/" + dataFileName;

        // 序列化对象
        string dataString = xs.SerializeObject(gameData, typeof(GameData));

        // 写入到文件中
        xs.CreateXML(gameDataFile, dataString);
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    void Load()
    {
        // 获取文件名
        string gameDataFile = GetDataPath() + "/" + dataFileName;

        // 判断文件是否存在
        if (File.Exists(gameDataFile))
        {
            // 加载文件
            string dataString = xs.LoadXML(gameDataFile);

            // 反序列化对象
            GameData gameDataFromXML = xs.DeserializeObject(dataString, typeof(GameData)) as GameData;

            // 是否是合法存档
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