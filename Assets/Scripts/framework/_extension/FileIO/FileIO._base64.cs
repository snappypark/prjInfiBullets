using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FileIO
{
    static class _base64
    {
        public static string Read(string folderPath, string fileName, string extension)
        {
            string fileurl = folderPath + "/" + fileName + "." + extension;
            //Debug.Log(GetFilePath(fileName, extension));
            //
            //FileInfo file = new FileInfo(GetFilePath(fileName, extension));
            FileInfo file = new FileInfo(fileurl);
            if (!file.Exists)
                return null;
            ///*
            byte[] byteArray = File.ReadAllBytes(fileurl);
            string result = Encoding.UTF8.GetString(byteArray);
            byte[] decodedBytes = Convert.FromBase64String(result);
            result = Encoding.UTF8.GetString(decodedBytes);
            //Debug.Log("[editor]" + result);
            return result;
            //*/

            /*
            Debug.Log("[editor]" + result);
            string xor = result;//XOR.Cipher(result);
            byte[] decodedBytes = Convert.FromBase64String(xor);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);
            return decodedText;
            //*/
            /*
            FileStream fs = new FileStream(GetFilePath(fileName, extension), FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            string parse = br.ReadString();
            br.Close();
            fs.Close();
            //Debug.Log("[editor]"+parse);
            return parse;
            //*/

            ///*
            //         string fileContents;
            ////         using (StreamReader reader = new StreamReader(fs))
            //             fileContents = reader.ReadToEnd();
        }

        public static void Write(string folderPath, string fileName, string context, string extension)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
                di.Create();
            string fileurl = folderPath + "/" + fileName + "." + extension;
            if (File.Exists(fileurl))
            {
                File.Delete(fileurl);
            }

            byte[] info = new UTF8Encoding(true).GetBytes(context);
            string endcode = Convert.ToBase64String(info);
            info = new UTF8Encoding(true).GetBytes(endcode);
            // byte[] info2 = new UTF8Encoding(true).GetBytes(xor);
            FileStream fs = new FileStream(fileurl, FileMode.OpenOrCreate, FileAccess.Write);
   
            fs.Write(info, 0, info.Length);
            /*
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(context);
            bw.Close();
            //*/

            /*
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            sw.WriteLine(context);
            sw.Close();*/
            fs.Close();
        }
    }
}