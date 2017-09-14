using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

/// <summary>
/// 保存和读取 xml 文件
/// </summary>
public class XmlSaver
{
    /// <summary>
    /// 内容加密
    /// </summary>
    /// <param name="toE">要加密的字符串</param>
    /// <returns>加密后的字符串</returns>
    public string Encrypt(string toE)
    {
        //加密和解密采用相同的key,具体自己填，但是必须为32位//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");

        // 生成对称加密管理对象
        RijndaelManaged rDel = new RijndaelManaged();

        // 设置密钥
        rDel.Key = keyArray;

        // 设置加密模式
        rDel.Mode = CipherMode.ECB;

        // 设置填充模式
        rDel.Padding = PaddingMode.PKCS7;

        // 生成对称加密对象
        ICryptoTransform cTransform = rDel.CreateEncryptor();

        // 把 字符串 编码为 utf-8
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toE);

        // 加密数据
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        // 把数据转换为 base64 编码
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }


    /// <summary>
    /// 内容解密
    /// </summary>
    /// <param name="toE">要解密的字符串</param>
    /// <returns>解密后的字符串</returns>
    public string Decrypt(string toD)
    {
        //加密和解密采用相同的key,具体值自己填，但是必须为32位//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");

        // 生成对称加密管理对象
        RijndaelManaged rDel = new RijndaelManaged();

        // 设置密钥
        rDel.Key = keyArray;

        // 设置加密模式
        rDel.Mode = CipherMode.ECB;

        // 设置填充模式
        rDel.Padding = PaddingMode.PKCS7;

        // 生成对称解密对象
        ICryptoTransform cTransform = rDel.CreateDecryptor();

        // 对数据进行 base64 解码
        byte[] toEncryptArray = Convert.FromBase64String(toD);

        // 解密数据
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        // 把 utf-8  编码转换回字符串
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    /// 序列化 对象
    /// </summary>
    /// <param name="pObject">要序列化的对象</param>
    /// <param name="ty">序列化的对象的类型</param>
    /// <returns></returns>
    public string SerializeObject(object pObject, System.Type ty)
    {
        MemoryStream memoryStream = new MemoryStream();

        // 生成 xml 序列化对象
        XmlSerializer xs = new XmlSerializer(ty);

        // 设置 xml 为 utf-8
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        // 序列化对象
        xs.Serialize(xmlTextWriter, pObject);

        // 获取序列后的数据流
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

        // 把字符串编码为 utf-8 
        return UTF8ByteArrayToString(memoryStream.ToArray());
    }

    /// <summary>
    /// 反序列化 对象
    /// </summary>
    /// <param name="pXmlizedString">要反序列化的字符串</param>
    /// <param name="ty">反序列化后对象的类型</param>
    /// <returns>反序列化后对象</returns>
    public object DeserializeObject(string pXmlizedString, System.Type ty)
    {
        // 创建 xml 序列化对象
        XmlSerializer xs = new XmlSerializer(ty);

        // 把字符串转换 utf-8 编码
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));

        // 设置 xml 为 utf-8 编码
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        // 反序列化对象
        return xs.Deserialize(memoryStream);
    }

    /// <summary>
    /// 创建XML文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="thisData">要保存的文件内容</param>
    public void CreateXML(string fileName, string thisData)
    {
        // 创建文件
        StreamWriter writer = File.CreateText(fileName);

        // 加密数据，并写入文件
        writer.Write(Encrypt(thisData));

        // 关闭文件
        writer.Close();
    }

    /// <summary>
    /// 读取XML文件
    /// </summary>
    /// <param name="fileName">要读取的文件名</param>
    /// <returns>文件的内容</returns>
    public string LoadXML(string fileName)
    {
        // 打开文件
        StreamReader sReader = File.OpenText(fileName);

        // 读取文件
        string dataString = sReader.ReadToEnd();

        // 关闭文件
        sReader.Close();

        // 返回解密后的内容
        return Decrypt(dataString);
    }


    /// <summary>
    /// 把utf-8 数组转换为 字符串
    /// </summary>
    /// <param name="characters">要转换的utf-8 数组</param>
    /// <returns>转换后 的 字符串</returns>
    public string UTF8ByteArrayToString(byte[] characters)
    {
        // 将 utf-8 编码 转换成字符串
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);

        // 返回字符串
        return (constructedString);
    }


    /// <summary>
    /// 把字符串转换为 utf-8 数组
    /// </summary>
    /// <param name="pXmlString">要转换的字符串</param>
    /// <returns>转换后 的 utf-8 数组</returns>
    public byte[] StringToUTF8ByteArray(String pXmlString)
    {
        // 对字符串进行 utf 编码
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);

        // 返回数组
        return byteArray;
    }
}