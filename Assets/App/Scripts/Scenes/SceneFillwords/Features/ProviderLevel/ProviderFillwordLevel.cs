using System.IO;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        public GridFillWords LoadModel(int index)
        {
            var allWords = File.ReadAllLines("Assets/App/Resources/Fillwords/words_list.txt");
            var levels = File.ReadAllLines("Assets/App/Resources/Fillwords/pack_0.txt");

            if (index > levels.Length) return null;

            var currentLevel = levels[index - 1].Split(' ');

            var gridSize = GetGridSize(currentLevel);
            var isValidate = ValidateLevel(gridSize, currentLevel, allWords);

            if (!isValidate || gridSize == 0) return null;

            var grid = FillGrid(gridSize, currentLevel, allWords);

            return grid;
        }

        private int GetGridSize(string[] currentLevel)
        {
            var gridSize = 1;
            var lettersCount = CountLetters(currentLevel);

            while (gridSize * gridSize < lettersCount)
            {
                gridSize++;
            }

            return gridSize * gridSize != lettersCount ? 0 : gridSize;
        }

        private int CountLetters(string[] currentLevel)
        {
            var lettersCount = 0;
            for (var i = 0; i < currentLevel.Length; i++)
            {
                var letterIndexes = currentLevel[i + 1].Split(';');
                for (var j = 0; j < letterIndexes.Length; j++)
                {
                    lettersCount++;
                }

                i++;
            }

            return lettersCount;
        }

        private bool ValidateLevel(int gridSize, string[] currentLevel, string[] allWords)
        {
            
            for (var i = 0; i < currentLevel.Length; i++)
            {
                var maxIndex = 0;
                var letterIndexes = currentLevel[i + 1].Split(';');
                var indexesCount = 0;
                foreach (var letterIndex in letterIndexes)
                {
                    var intIndex = int.Parse(letterIndex);
                    if (intIndex >= maxIndex) maxIndex = intIndex;
                    indexesCount++;
                }

                if (allWords[int.Parse(currentLevel[i])].Length != indexesCount || maxIndex > gridSize * gridSize)
                    return false;

                i++;
            }

            return true;
        }

        private GridFillWords FillGrid(int gridSize, string[] currentLevel, string[] allWords)
        {
            var grid = new GridFillWords(new Vector2Int(gridSize, gridSize));

            for (var i = 0; i < currentLevel.Length; i++)
            {
                var word = allWords[int.Parse(currentLevel[i])];
                var letterIndexes = currentLevel[i + 1].Split(';');

                for (var j = 0; j < letterIndexes.Length; j++)
                {
                    var letter = word[j];
                    var letterIndex = int.Parse(letterIndexes[j]);

                    var row = letterIndex / gridSize;
                    var col = letterIndex % gridSize;

                    grid.Set(row, col, new CharGridModel(letter));
                }

                i++;
            }

            return grid;
        }
    }
}