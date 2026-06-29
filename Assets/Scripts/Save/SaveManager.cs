using System;
using System.IO;
using UnityEngine;

namespace IdleFarm.Save {
    public static class SaveManager {
        private const string SaveFileName = "save.json";
        public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        public static bool HasSave =>  File.Exists(SavePath);

        public static bool Save(SaveData data) {
            if (data == null) 
                return false;

            try {
                var directory = Path.GetDirectoryName(SavePath);
                if (!string.IsNullOrEmpty(directory)) {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);

                return true;
            }
            catch (Exception exception) {
                Debug.LogWarning($"Save failed: {exception.Message}");
                return false;
            }
        }

        public static bool TryLoad(out SaveData data) {
            data = default;
            if (!HasSave) {
                return false;
            }

            try {
                var json = File.ReadAllText(SavePath);
                data = JsonUtility.FromJson<SaveData>(json);
                return true;
            }
            catch (Exception exception) {
                Debug.LogWarning($"Load failed: {exception.Message}");
                data = default;
                return false;
            }
        }

        public static bool DeleteSave() {
            if (!HasSave) {
                return false;
            }

            try {
                File.Delete(SavePath);
                return true;
            }
            catch (Exception exception) {
                Debug.LogWarning($"Delete save failed: {exception.Message}");
                return false;
            }
        }
    }
}