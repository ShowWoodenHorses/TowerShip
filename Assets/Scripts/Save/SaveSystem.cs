using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Configs;
using Assets.Scripts.Save;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private static DefaultSaveConfig defaultConfig =
        Resources.Load<DefaultSaveConfig>("DefaultSaveConfig");

    // Создаём сейв с дефолтами
    public static SaveData CreateDefaultSave()
    {
        return new SaveData()
        {
            version = 3,
            currentCoins = defaultConfig.currentCoins,
            allCoins = defaultConfig.allCoins,
            health = defaultConfig.health,
            selectedPLayerId = defaultConfig.selectedPLayerId,
            selectedBulletId = defaultConfig.selectedBulletId,
            currentWaveEnemyId = defaultConfig.currentWaveEnemyId,
            ownedItems = new List<string>(defaultConfig.ownedItems),
            ownedTowersDict = new List<SaveTileData>(defaultConfig.ownedTowersDict),
            ownedGunStatDict = new List<SaveGunStatData>(defaultConfig.ownedGunStatDict),
        };
    }

    // Сохранение
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Saved to {SavePath}");

#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLStorageHelper.Flush(); // синхронизируем изменения в IndexedDB
#endif
    }

    // Создание нового сейва с дефолтными значениями
    public static void New()
    {
        var fresh = CreateDefaultSave();
        Save(fresh);
    }

    public static bool IsExistsSave()
    {
        return File.Exists(SavePath);
    }

    // Полное удаление сейва
    public static void DeleteSave()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[SaveSystem] Save deleted");
#if UNITY_WEBGL && !UNITY_EDITOR
                WebGLStorageHelper.Flush(); // фиксируем удаление в IndexedDB
#endif
            }
            else
            {
                Debug.Log("[SaveSystem] No save file to delete");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveSystem] Failed to delete save: {e.Message}");
        }
    }

    // Полный сброс (удаление + создание нового)
    public static void ResetSave()
    {
        DeleteSave();
        New();
        Debug.Log("[SaveSystem] Save reset to defaults");
    }

    // Загрузка сейва
    public static SaveData Load()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveSystem] No save found → creating default");
                var fresh = CreateDefaultSave();
                Save(fresh);
                return fresh;
            }

            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
                throw new Exception("JsonUtility returned null");

            return Migrate(data);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveSystem] Load failed: {e.Message}");
            var fallback = CreateDefaultSave();
            Save(fallback);
            return fallback;
        }
    }

    // Миграция старых сейвов
    private static SaveData Migrate(SaveData data)
    {
        if (data.version < 2)
        {
            Debug.Log("[SaveSystem] Migrating save from v1 → v2");

            if (data.ownedItems == null || data.ownedItems.Count == 0)
            {
                data.ownedItems = new List<string>();

                if (!string.IsNullOrEmpty(data.selectedPLayerId))
                    data.ownedItems.Add(data.selectedPLayerId);
            }

            data.version = 2;
            Save(data);
        }

        return data;
    }
}
