
using System;
using UnityEngine;
using Newtonsoft.Json;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            string jsonFilePath = "WordSearch/Levels/" + levelIndex;

            TextAsset jsonTextAsset = Resources.Load<TextAsset>(jsonFilePath);

            if (jsonTextAsset != null)
            {
                try
                {
                    string jsonText = jsonTextAsset.text;
                    var levelData = JsonConvert.DeserializeObject<LevelInfo>(jsonText);
                    return levelData;
                }
                catch (Exception ex)
                {
                    Debug.Log("Ошибка при загрузке уровня: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("Файл уровня не найден: " + jsonFilePath);
            }
            return null;
        }
    }
}
