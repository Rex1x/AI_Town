using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class DataManager
{
    private static string dataPath = Path.Combine(Application.persistentDataPath, "ResidentsData");

    // 주민 데이터 저장
    public static void SaveAllResidents(Resident[] residents)
    {
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        foreach (var resident in residents)
        {
            string filePath = Path.Combine(dataPath, $"{resident.RID}.json");
            string jsonData = JsonConvert.SerializeObject(resident);
            File.WriteAllText(filePath, jsonData);
        }
    }

    // 저장된 주민 데이터 불러오기
    public static Resident[] LoadAllResidents()
    {
        if (!Directory.Exists(dataPath))
        {
            return new Resident[0];
        }

        string[] files = Directory.GetFiles(dataPath, "*.json");
        List<Resident> residents = new List<Resident>();

        foreach (string file in files)
        {
            string jsonData = File.ReadAllText(file);
            Resident resident = JsonConvert.DeserializeObject<Resident>(jsonData);
            residents.Add(resident);
        }

        return residents.ToArray();
    }

    // 저장된 주민 데이터 삭제 (테스트나 리셋 시 사용)
    public static void ClearAllResidentData()
    {
        if (Directory.Exists(dataPath))
        {
            Directory.Delete(dataPath, true);
        }
    }
}
