using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            HashSet<char> uniqueChars = new();
            
            foreach (var letter in words.SelectMany(word => word))
            {
                uniqueChars.Add(letter);
            }

            var charCount = new Dictionary<char, int>();
            
            foreach (var letter in uniqueChars)
            {
                var maxCount = 0;
                foreach (var word in words)
                {
                    var countInWord = word.Count(ch => ch == letter);
                    maxCount = Math.Max(maxCount, countInWord);
                }

                charCount[letter] = maxCount;
            }

            var charsList = new List<char>();

            foreach (var kvp in charCount)
            {
                for (var i = 0; i < charCount[kvp.Key]; i++)
                {
                    charsList.Add(kvp.Key);
                }
            }

            foreach (var letter in uniqueChars.Where(letter => !charsList.Contains(letter)))
            {
                charsList.Add(letter);
            }
            
            return charsList;
        }
    }
}