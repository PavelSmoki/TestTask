using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            var jsonFile = (TextAsset)Resources.Load($"WordSearch/Levels/{levelIndex}");
            var levelInfo = JsonUtility.FromJson<LevelInfo>(jsonFile.text);

            return levelInfo;
        }
    }
}