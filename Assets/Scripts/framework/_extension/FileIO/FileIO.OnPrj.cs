using System.IO;
using UnityEngine;

namespace FileIO
{
    public static class OnPrj
    {
        public static void CreateFile_stage(string fileName, string context, string extension)
        {
            _base64.Write(Application.dataPath + "/Resources_/_baked/stages", fileName, context, extension);
            Debug.Log(Time.time +" [Saved Edit File] " + fileName); 
        }
        public static string ReadStage(string fileName, string extension)
        {
            return _base64.Read(Application.dataPath + "/Resources_/_baked/stages", fileName, extension);
        }
        
        public static void CreateFile_prjss(string context, string extension)
        {
            _base64.Write(Application.dataPath + "/Resources_/_baked", "prjss", context, extension);
        }
        
        public static void CreateFile_sprite(string fileName, byte[] bytes)
        {
            string filePath = Application.dataPath + "/Resources_/_baked/pngs/";
            File.WriteAllBytes(filePath + fileName, bytes);
        }

        
    }
}
