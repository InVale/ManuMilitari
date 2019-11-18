using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [NonSerialized] public int PlayerNumber;

    List<Order[][]> TurnLogs = new List<Order[][]>();

    void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    public void LogTurn(List<Player> players)
    {
        TurnLogs.Add(new Order[players.Count][]);
        for (int i = 0; i < players.Count; i++)
        {
            TurnLogs[TurnLogs.Count - 1][i] = new Order[players[i].Units.Count];
            for (int j = 0; j < players[i].Units.Count; j++)
            {
                //TurnLogs[TurnLogs.Count - 1][i][j] = players[i].Units[j].UnitOrder;
            }
        }
    }
    /*
    public static T LoadData<T>(string filename, T defaultValue)
    {

        if (!File.Exists(Application.persistentDataPath + '/' + filename))
        {
            return defaultValue;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + '/' + filename, FileMode.Open);
        string encryptedData = (string)bf.Deserialize(file);
        string decryptedData = XOREncryptor.EncryptDecrypt(encryptedData);
        T returnValue = JsonUtility.FromJson<T>(decryptedData);
        file.Close();
        return returnValue;
    }

    public static bool SaveData<T>(string filename, T data)
    {

        if (File.Exists(Application.persistentDataPath + '/' + filename))
        {
            File.Delete(Application.persistentDataPath + '/' + filename);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + '/' + filename);
        string decryptedData = JsonUtility.ToJson(data);
        string encryptedData = XOREncryptor.EncryptDecrypt(decryptedData);
        bf.Serialize(file, encryptedData);
        file.Close();
        return true;
    }
    */
}
