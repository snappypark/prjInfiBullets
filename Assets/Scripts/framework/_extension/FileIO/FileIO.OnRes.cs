using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FileIO
{
    public static class OnRes
    {
        public static void CreateFile_stage(string fileName, string context, string extension)
        {
            _base64.Write(Application.dataPath + "/Resources_/_baked/stages/_", fileName, context, extension);
            Debug.Log("[Saved Edit File] " + fileName); 
        }

        public static string ReadStage(string fileName, string extension)
        {
            return _base64.Read(Application.dataPath + "/Resources_/_baked/stages/_", fileName, extension);
        }

        public static string Pasing(string path)
        {
            //Debug.Log(path);
            TextAsset textAsset = Resources.Load(path) as TextAsset;
            
            MemoryStream s = new MemoryStream(textAsset.bytes);
            BinaryReader br = new BinaryReader(s);
            string parse = br.ReadString();
            br.Close();
            s.Close();
            //Debug.Log(parse);
            return parse;
        }


        static string NameOfSubFolor(int idx)
        {
            int d = (int)(idx / 100);
            switch (d)
            {
                case 0: return "001";
            }
            return string.Format("{0}", d*100);
        }
    }

}


