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
            var uniqueChars = FindUniqueChars(words);
            var charCount = FindMostCommonChars(words, uniqueChars);
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

        private HashSet<char> FindUniqueChars(List<string> words)
        {
            HashSet<char> uniqueChars = new();

            foreach (var letter in words.SelectMany(word => word))
            {
                uniqueChars.Add(letter);
            }

            return uniqueChars;
        }

        private Dictionary<char, int> FindMostCommonChars(List<string> words, HashSet<char> uniqueChars)
        {
            var charCount = new Dictionary<char, int>();

            foreach (var letter in uniqueChars)
            {
                var maxCount = words.Select(word => word.Count(ch => ch == letter)).Prepend(0).Max();
                charCount[letter] = maxCount;
            }

            return charCount;
        }
    }
}