using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZENC.CORE.Cryptography;
using ZENC.CORE.Util;

namespace ZENC.CORE
{
    public static class ZENCExtension
    {
        public static string EzCreateDirectory(this string directoryFullPath)
        {
            return FileUtil.CreateDirectory(directoryFullPath);
        }


        public static bool EzBool(this object obj)
        {
            bool isSuccess = false;

            return obj.EzBool(out isSuccess);
        }
        public static bool EzBool(this object obj, out bool isSuccess)
        {
            bool returnValue = false;

            isSuccess = bool.TryParse(obj.EzToString(), out returnValue);

            if (!isSuccess && obj.EzToString().EzNotNullOrEmpty())
                returnValue = Convert.ToBoolean(obj.EzToString().EzInt());

            return returnValue;
        }
        public static int EzInt(this object obj)
        {
            bool isSuccess = false;
            return obj.EzInt(out isSuccess);
        }

        public static float EzFloat(this object obj)
        {
            bool isSuccess = false;
            return obj.EzFloat(out isSuccess);
        }

        public static double EzDouble(this object obj)
        {
            bool isSuccess = false;
            return obj.EzDouble(out isSuccess);
        }
        public static long EzLong(this object obj)
        {
            bool isSuccess = false;
            return obj.EzLong(out isSuccess);
        }

        public static T EzEnum<T>(this string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static T EzEnum<T>(this string str, out bool isConvert)
        {
            try
            {
                isConvert = true;
                return (T)Enum.Parse(typeof(T), str);
            }
            catch
            {
                isConvert = false;
                return default(T);
            }
        }

        public static int EzInt(this object obj, out bool isSuccess)
        {
            int returnValue = 0; // 

            if (obj == null)
            {
                isSuccess = false;
                return 0;
            }

            if (obj != null && obj is bool)
            {
                isSuccess = true;

                bool value = (bool)obj;
                if (value) return 1;
                else return 0;
            }

            isSuccess = int.TryParse(obj.EzToString(), out returnValue);

            return returnValue;
        }

        public static float EzFloat(this object obj, out bool isSuccess)
        {
            float returnValue = -1f;
            isSuccess = float.TryParse(obj.EzToString(), out returnValue);
            return returnValue;
        }

        public static double EzDouble(this object obj, out bool isSuccess)
        {
            double returnValue = -1f;
            isSuccess = double.TryParse(obj.EzToString(), out returnValue);
            return returnValue;
        }

        public static long EzLong(this object obj, out bool isSuccess)
        {
            long returnValue = 0;
            isSuccess = long.TryParse(obj.EzToString(), out returnValue);
            return returnValue;
        }


        public static string EzCombine(this string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }
        public static bool EzIsNull<T>(this T obj)
        {

            if (obj == null)
                return true;
            else
                return false;

        }

        public static bool EzNotNull<T>(this T obj)
        {

            if (obj == null)
                return false;
            else
                return true;


        }
        public static bool EzNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str.EzToString().Trim());
        }

        public static bool EzIsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string EzToString(this string obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString();
        }
        public static string EzToString(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString();
        }
        public static string EzToLower(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString().ToLower();
        }
        public static string EzToUpper(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString().ToUpper();
        }

        public static string EzToString(this DateTime dt, string format)
        {

            return dt.ToString(format);
        }

        public static string EzBase64String(this byte[] buffer)
        {
            if(buffer.EzNotNull())
            return Convert.ToBase64String(buffer);

            return string.Empty;
        }

        public static byte[] EzBase64Byte(this string str)
        {
            if(str.EzNotNullOrEmpty())
            return Convert.FromBase64String(str);

            return null;
        }

        public static byte[] EzASCIIByte(this string str)
        {
            if (str.EzNotNullOrEmpty())
                return Encoding.ASCII.GetBytes(str);

            return null;
        }
        public static Stream EzASCIIStream(this string str)
        {
            if (str.EzNotNullOrEmpty())
                return new MemoryStream(str.EzASCIIByte());

            return null;
        }
        public static string EzASCIIString(this byte[] buffer)
        {
            if (buffer != null)
                return Encoding.ASCII.GetString(buffer);
            else
            {
                return string.Empty;
            }
        }

        public static string EzASCIIString(this Stream strm)
        {
            if (strm != null)
            {

                byte[] buffer = new byte[strm.Length];
                strm.Read(buffer, 0, buffer.Length);
                return buffer.EzASCIIString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static string EzDirectoryName(this string fileName)
        {
            if (fileName.EzIsFile())
            {
                try
                {
                    return System.IO.Path.GetDirectoryName(fileName);
                }
                catch (PathTooLongException)
                {
                    return fileName.Substring(0, fileName.LastIndexOf("\\"));// System.IO.Path.GetDirectoryName(fileName);
                }
            }
            else
                return fileName;
        }

        public static string EzFileExtension(this string fileName)
        {
            return System.IO.Path.GetExtension(fileName);
        }

        public static string EzFileNameWithoutExtension(this string fileName)
        {
            string str = System.IO.Path.GetFileName(fileName);
            return System.IO.Path.GetFileNameWithoutExtension(str);
        }

        public static string EzFileName(this string fileName)
        {
            return System.IO.Path.GetFileName(fileName);
        }

        public static bool EzIsExists(this string path)
        {
            return FileUtil.IsExists(path);
        }

       
        public static bool EzFileDelete(this string filePath)
        {
            try
            {

                if (filePath.EzNotNullOrEmpty())
                {
                    FileInfo file = new FileInfo(filePath);


                    if (file.Exists)
                    {
                        if (file.IsReadOnly)
                            file.IsReadOnly = false;

                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {

                        }

                        int loopCount = 0;
                        while (FileUtil.IsFileLocked(file)) // 만약 파일이 잠겨있다면 루프를 도는데
                        {
                            if (loopCount >= 13) // 3초까지만 돕니다.
                            {
                                break; // 그냥 탈출
                            }
                            System.Threading.Thread.Sleep(1000);
                            loopCount++;

                        }

                        return FileUtil.RunWipe(filePath);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static List<FileInfo> EzFileList(this string path)
        {
            if (path.EzIsExists())
            {
                if (path.EzIsDirectory())
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    FileInfo[] fileArray = dir.GetFiles();
                    if (fileArray.EzNotNull() && fileArray.Length > 0)
                    {
                        return fileArray.ToList();
                    }
                }
            }

            return null;
        }
        public static List<DirectoryInfo> EzDirectoryList(this string path)
        {
            if (path.EzIsExists())
            {
                if (path.EzIsDirectory())
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    DirectoryInfo[] dirArray = dir.GetDirectories();
                    if (dirArray.EzNotNull() && dirArray.Length > 0)
                    {
                        return dirArray.ToList();
                    }
                }
            }

            return null;
        }
        public static bool EzIsFile(this string path)
        {
            return FileUtil.IsFile(path);
        }
        public static bool EzIsDirectory(this string path)
        {
            return FileUtil.IsDirectory(path);
        }
        public static string EzEncryptString(this string str)
        {
            if (str.EzNotNullOrEmpty())
            {
                ICryptography cry = ZENC.CORE.Cryptography.CrypEnvironment.Get(CORE.Cryptography.CrypType.AES);
                return cry.EncryptString(str);
            }
            return str;
        }

        public static string EzDecryptString(this string str)
        {
            if (str.EzNotNullOrEmpty())
            {
                ICryptography cry = ZENC.CORE.Cryptography.CrypEnvironment.Get(CORE.Cryptography.CrypType.AES);
                return cry.DecryptString(str);
            }
            return str;
        }


        public static void EzEncryptFile(this string path)
        {
            if (path.EzNotNullOrEmpty() && path.EzIsFile())
            {
                ICryptography cry = ZENC.CORE.Cryptography.CrypEnvironment.Get(CORE.Cryptography.CrypType.AES);
                cry.EncryptFile(path);
            }
        }

        public static void EzDecryptFile(this string path)
        {
            if (path.EzNotNullOrEmpty() && path.EzIsFile())
            {
                ICryptography cry = ZENC.CORE.Cryptography.CrypEnvironment.Get(CORE.Cryptography.CrypType.AES);

                cry.DecryptFile(path);
            }

        }


        public static T EzJsonDeserialize<T>(this string obj)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(obj);
        }

        public static string EzJsonSerialize(this object obj)
        {
            if(obj.EzNotNull())
            return System.Text.Json.JsonSerializer.Serialize(obj);

            return null;
        }
    }
}
