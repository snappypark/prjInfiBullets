using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class XOR
{ 
    public static string Cipher(string data, string key = "rpo-l")
    {
        int dataLen = data.Length;
        int keyLen = key.Length;
        char[] output = new char[dataLen];

        for (int i = 0; i < dataLen; ++i)
        {
            output[i] = (char)(data[i] ^ key[i % keyLen]);
        }

        return new string(output);
    }
}

namespace FileIO
{
    public static class OnLocal
    {
        static string GetFolderPath() { return string.Format("{0}/js", Application.persistentDataPath); }
        static string GetFilePath(string fileName, string extension) { return string.Format("{0}/{1}.{2}", GetFolderPath(), fileName, extension); }

        public static FileInfo[] GetSaveFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(GetFolderPath());
            if (dir.Exists == false)
                return null;
            return dir.GetFiles("*.txt", SearchOption.AllDirectories);
        }

        public static List<string> GetSaveFileNames()
        {
            FileInfo[] files = GetSaveFiles();
            if (null == files)
                return null;

            List<string> lstName = new List<string>();
            for (int i = 0; i < files.Length; ++i)
                lstName.Add(files[i].Name.Substring(0, files[i].Name.LastIndexOf('.')));
            return lstName;
        }

        public static string Read(string fileName, string extension)
        {
            return _base64.Read(GetFolderPath(), fileName, extension);
        }
        
        public static void WriteNew(string fileName, string context, string extension)
        {
            _base64.Write(GetFolderPath(), fileName, context, extension);
        }
        
        public static void Delete(string fileName)
        {
            FileInfo[] files = GetSaveFiles();
            if (null == files)
                return;

            for (int i = 0; i < files.Length; ++i)
                if (files[i].Name.Equals(string.Format("{0}.txt", fileName)))
                {
                    files[i].Delete();
                    return;
                }
        }




    }

}
