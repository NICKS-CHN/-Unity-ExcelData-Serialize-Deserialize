using System;
using System.IO;
using UnityEngine;

namespace AssetPipeline
{
    public class FileHelper
    {
        public static bool IsExist(string path)
        {
            return File.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static byte[] ReadAllBytes(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }

        public static string ReadAllText(string path)
        {
            try
            {
                string 
                    data = File.ReadAllText(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return null;
        }


        public static void WriteAllBytes(string path, ByteArray byteArray)
        {
            WriteAllBytes(path, byteArray.bytes);
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            string dir = Path.GetDirectoryName(path);
            CreateDirectory(dir);

            File.WriteAllBytes(path, bytes);
        }

        public static ByteArray LoadByteArrayFromFile(string filePath)
        {
            var bytes = ReadAllBytes(filePath);
            if (bytes != null)
            {
                return new ByteArray(bytes);
            }
            return null;
        }

    }
}
