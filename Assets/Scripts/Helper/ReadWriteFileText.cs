using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;

#if NETFX_CORE
 using System.Threading.Tasks;
 using Windows.Storage;
 using Windows.Storage.Streams;
#endif


public class ReadWriteFileText
{
    public static void WriteFile(string content, string path)
    {
        StreamWriter swriter = new StreamWriter(path, false);
        swriter.Write(content);
        swriter.Flush();
        swriter.Close();
    }
    public static string ReadFile(string path)
    {
        string str = "";
        StreamReader sreader = new StreamReader(path);
        str = sreader.ReadToEnd();
        sreader.Close();
        return str;
    }


    //=================
    public static void WriteDefaultValues(string fileName, string Content)
    {
        // Check directory exists
        //IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        //if (isoStore.GetFileNames(fileName).Length > 0)
        //{
        //    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore))
        //    {
        //        using (StreamWriter writer = new StreamWriter(isoStream))
        //        {
        //            writer.WriteLine(Content);
        //        }
        //    }
        //}
        //else
        //{
        //    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.CreateNew, isoStore))
        //    {
        //        using (StreamWriter writer = new StreamWriter(isoStream))
        //        {
        //            writer.WriteLine(Content);
        //        }
        //    }
        //}
    }

    public static string DisplayValues(string fileName)
    {
        string str = "";
        // Check directory exists
        //IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        //if (isoStore.GetFileNames(fileName).Length > 0)
        //{
        //    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))
        //    {
        //        using (StreamReader reader = new StreamReader(isoStream))
        //        {
        //            str = reader.ReadToEnd();
        //        }
        //    }
        //}
        return str;
    }

    #region============READ AND SAVE STRING TO PLAYERPREFS==========
    public static void SaveStringToPrefab(string key,string content)
    {
        PlayerPrefs.SetString(key, content);
        PlayerPrefs.Save();
    }
    public static string GetStringFromPrefab(string key)
    {
        string result="";
        if (PlayerPrefs.HasKey(key))
        {
            result = PlayerPrefs.GetString(key);
        }
        return result;
    }
    #endregion
}

