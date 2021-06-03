using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoadData : MonoBehaviour
{
    public static Data data;

    private static bool appIsStarting = true;

    void Awake()
    {
        try
        {
            if (appIsStarting)
            {
                data = new Data();
                LoadFromFile();
            }
            appIsStarting = false;
        }
        catch
        {
            data = new Data();
            SaveToFile();
            LoadFromFile();
        }
        
    }

                                                            // Saves in a file the 'data' structure which contains
                                                            // all the characteritics of the user session. Like:
                                                            // coins, highscore, maxFuel, skins, superPowers, etc.
                                                            // **** IMPORTANT **** Before using this method, stash the
                                                            // values that changed and should be saved. Only stashed
                                                            // data will be saved.
    public static void SaveToFile()
    {
        string filepath = Application.persistentDataPath + "/appSave.dat";

        using (FileStream file = File.Create(filepath))
        {
            new BinaryFormatter().Serialize(file, data);
        }
    }

    public static void LoadFromFile()
    {

        string filepath = Application.persistentDataPath + "/appSave.dat";

        using (FileStream file = File.Open(filepath, FileMode.Open))
        {
            object loadedData = new BinaryFormatter().Deserialize(file);
            data = (Data)loadedData;
        }

        Debug.Log("Loaded the fav list: " + data.favoriteIdList);
    }

    public static void stashFavoriteIdList(HashSet<string> idList)
    {
        data.favoriteIdList = idList;
    }

}

[Serializable]
public class Data
{
    public HashSet<string> favoriteIdList = new HashSet<string>();
}