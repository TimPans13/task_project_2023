using App.Scripts.Infrastructure.LevelSelection;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using App.Scripts.Scenes.SceneFillwords.States.Setup;
using System;
using System.IO;
using System.Linq;
using UnityEngine;


namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private readonly string dictionaryFilePath = "Fillwords/words_list"; 
        private readonly string levelsFilePath = "Fillwords/pack_0"; 

        public delegate void LoadModelDelegate(int index);
        public event Action<int> OnLoadModelRequested;

        public GridFillWords LoadModel(int index)
        {
            TextAsset dictionaryTextAsset = Resources.Load<TextAsset>(dictionaryFilePath);
            TextAsset levelsTextAsset = Resources.Load<TextAsset>(levelsFilePath);

            if (dictionaryTextAsset == null || levelsTextAsset == null)
            {
                Debug.LogError("Не удалось загрузить файлы словаря или уровней.");
                return null;
            }

            string[] dictionary = dictionaryTextAsset.text
    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
    .Select(line => line.Trim())
    .ToArray();
            string[] levels = levelsTextAsset.text.Split('\n');


            if (index < 0 || index >= levels.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid level index.");
            }

            string levelData = levels[index];

            if (string.IsNullOrEmpty(levelData))
            {
                return null;
            }

            var levelParts = levelData.Split(' ');

            if (levelParts.Length < 2)
            {
                return null;
            }

            var wordIndexes = levelParts
                .Where(s => int.TryParse(s, out _))
                .Select(int.Parse)
                .ToArray();

            var levelDescriptionParts = levelParts
                .Where(s => !int.TryParse(s, out _))
                .ToArray();

            if (wordIndexes.Length < 1 || levelDescriptionParts.Length < 1)
            {
                return null;
            }

            var levelDescription = string.Join(";", levelDescriptionParts);
            var gridSize = (int)Math.Sqrt(levelDescription.Count(c => c == ';') + 1);
            var words = wordIndexes.Select(index => dictionary[index]).ToArray();

            char[] wordsArr = ToArr(words);
            int[] levelDescriptionArr = ToArrInt(levelDescription);


            if (!ValidateLevel(wordsArr, levelDescriptionArr))
            {
                OnLoadModelRequested?.Invoke(index + 1);
                return LoadModel(index + 1);
            }

            wordsArr = Sort(wordsArr, levelDescriptionArr);
            var grid = ParseGrid(levelDescription, gridSize, wordsArr);
            var gridFillWords = new GridFillWords(new Vector2Int(gridSize, gridSize));

            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    gridFillWords.Set(i, j, grid[i][j]);
                }
            }

            return gridFillWords;
        }

        static bool IsSquare(int number)
        {
            int squareRoot = (int)Math.Sqrt(number);
            return squareRoot * squareRoot == number;
        }

        private char[] Sort(char[] letters, int[] adresses)
        {
            int n = letters.Length;
            char[] temp = new char[n];

            for (int i = 0; i < n; i++)
            {
                temp[adresses[i]] = letters[i];
            }

            Array.Copy(temp, letters, n);
            return letters;
        }

        private int[] ToArrInt(string input)
        {
            char separator = ';';
            string[] numberStrings = input.Split(separator);
            int[] numbers = new int[numberStrings.Length];

            for (int i = 0; i < numberStrings.Length; i++)
            {
                if (int.TryParse(numberStrings[i], out int number))
                {
                    numbers[i] = number;
                }
                else
                {
                    Debug.Log($"Ошибка преобразования: {numberStrings[i]} не является целым числом.");
                }
            }

            return numbers;
        }

        private char[] ToArr(string[] words)
        {
            int totalLength = 0;
            foreach (string word in words)
            {
                totalLength += word.Length;
            }

            char[] result = new char[totalLength];

            int currentIndex = 0;
            foreach (string word in words)
            {
                foreach (char c in word)
                {
                    result[currentIndex] = c;
                    currentIndex++;
                }
            }

            return result;
        }

        private bool ValidateLevel(char[] wordsArr, int[] levelDescriptionArr)
        {

            if (!(IsSquare(wordsArr.Length) && IsSquare(levelDescriptionArr.Length)))
            {
                return false;
            }

            bool areAllUnique = levelDescriptionArr.Distinct().Count() == levelDescriptionArr.Length;
            if (!areAllUnique)
            {
                return false;
            }
            int max = levelDescriptionArr.Max();
            int min = levelDescriptionArr.Min();
            if (!((max == levelDescriptionArr.Length) || (min == 0)))
            {
                return false;
            }

            if (!(wordsArr.Length == levelDescriptionArr.Length))
            {
                return false;
            }

            return true;
        }

        private CharGridModel[][] ParseGrid(string levelDescription, int gridSize, char[] wordsArr)
        {
            var grid = new CharGridModel[gridSize][];
            var gridIndexes = levelDescription.Split(';').Select(int.Parse).ToArray();
            var charIndex = 0;

            for (var i = 0; i < gridSize; i++)
            {
                grid[i] = new CharGridModel[gridSize];
                for (var j = 0; j < gridSize; j++)
                {
                    grid[i][j] = new CharGridModel(' ');
                    charIndex++;
                }
            }

            for (int i = 0; i < wordsArr.Length; i++)
            {
                int row = i / grid.Length;
                int col = i % grid.Length;

                grid[row][col] = new CharGridModel(wordsArr[i]);
            }

            return grid;
        }
    }
}