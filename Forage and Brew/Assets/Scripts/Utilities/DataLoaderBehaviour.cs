using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#if UNITY_EDITOR

public class DataLoaderBehaviour : MonoBehaviour
{
    [SerializeField] private Object narrativeBlockDataCsv;
    [SerializeField] private Object letterDataCsv;
    [SerializeField] private Object recipeDataCsv;
    [SerializeField] private Object ingredientDataCsv;
    
    [SerializeField] private GameDontDestroyOnLoadManager gameDontDestroyOnLoadManager;
    [SerializeField] private Object narrativeBlockDataFolder;
    private string _narrativeBlockDataFolderPath;
    [SerializeField] private Object clientDataFolder;
    private string _clientDataFolderPath;
    private List<ClientSo> _clients;
    [SerializeField] private Object letterDataFolder;
    private string _letterDataFolderPath;
    [SerializeField] private Object orderDataFolder;
    private string _orderDataFolderPath;
    [SerializeField] private Object ingredientDataFolder;
    private string _ingredientDataFolderPath;
    private List<IngredientValuesSo> _ingredients;
    [SerializeField] private Object potionDataFolder;
    private string _potionDataFolderPath;
    private List<PotionValuesSo> _potions;
    [SerializeField] private Object potionDifficultyDataFolder;
    private List<PotionDifficultySo> _potionDifficulties;
    
    [SerializeField] private List<WeatherStateSo> weatherStates;
    [SerializeField] private List<LunarCycleStateSo> lunarCycleStates;
    [SerializeField] private IngredientListSo ingredientListSo;
    [SerializeField] private PotionListSo potionListSo;
    [SerializeField] private ChoppingHapticChallengeListSo choppingHapticChallengeListSo;
    [SerializeField] private GrindingHapticChallengeSo grindingHapticChallengeSo;
    [SerializeField] private StirHapticChallengeSo stirHapticChallengeSo;
    
    
    [Button("Generate Data")]
    private void GenerateData()
    {
        _narrativeBlockDataFolderPath = AssetDatabase.GetAssetPath(narrativeBlockDataFolder);
        _clientDataFolderPath = AssetDatabase.GetAssetPath(clientDataFolder);
        _clients = Directory.GetFiles(_clientDataFolderPath).Select(AssetDatabase.LoadAssetAtPath<ClientSo>).Where(x => x != null).ToList();
        _letterDataFolderPath = AssetDatabase.GetAssetPath(letterDataFolder);
        _orderDataFolderPath = AssetDatabase.GetAssetPath(orderDataFolder);
        _ingredientDataFolderPath = AssetDatabase.GetAssetPath(ingredientDataFolder);
        _ingredients = Directory.GetFiles(_ingredientDataFolderPath).Select(AssetDatabase.LoadAssetAtPath<IngredientValuesSo>).Where(x => x != null).ToList();
        _potionDataFolderPath = AssetDatabase.GetAssetPath(potionDataFolder);
        _potions = Directory.GetFiles(_potionDataFolderPath).Select(AssetDatabase.LoadAssetAtPath<PotionValuesSo>).Where(x => x != null).ToList();
        _potionDifficulties = Directory.GetFiles(AssetDatabase.GetAssetPath(potionDifficultyDataFolder))
            .Select(AssetDatabase.LoadAssetAtPath<PotionDifficultySo>).Where(x => x != null).ToList();
        
        foreach (var path in new[] {_narrativeBlockDataFolderPath, _letterDataFolderPath, _orderDataFolderPath})
        {
            foreach (var file in AssetDatabase.FindAssets("t:ScriptableObject", new[] {path}).Select(AssetDatabase.GUIDToAssetPath))
            {
                AssetDatabase.DeleteAsset(file);
            }
        }
        
        gameDontDestroyOnLoadManager.AllNarrativeBlocksContentSo.Clear();
        
        // Narrative Blocks Csv
        TextAsset csvFile = Resources.Load<TextAsset>(narrativeBlockDataCsv.name);
        GenerateCsvElementList(csvFile, out List<List<string>> narrativeBlockCsvElementList);
        
        foreach (List<string> lineData in narrativeBlockCsvElementList)
        {
            if (lineData[0] == "") continue; // Skip empty lines

            NarrativeBlockOfLettersContentSo narrativeBlockInstance = ScriptableObject.CreateInstance<NarrativeBlockOfLettersContentSo>();
            int requiredQuestProgressionIndex = int.Parse(lineData[1]);
            narrativeBlockInstance.SetData(lineData[0], requiredQuestProgressionIndex);
            
            string newNarrativeBlockDataPath = _narrativeBlockDataFolderPath + "/NB" +
                                               requiredQuestProgressionIndex + "_" + lineData[0].Replace(" ", "") + ".asset";
            AssetDatabase.CreateAsset(narrativeBlockInstance, newNarrativeBlockDataPath);
            
            gameDontDestroyOnLoadManager.AllNarrativeBlocksContentSo.Add(narrativeBlockInstance);
        }
        
        // Ingredients Csv
        csvFile = Resources.Load<TextAsset>(ingredientDataCsv.name);
        GenerateCsvElementList(csvFile, out List<List<string>> ingredientCsvElementList);
        
        foreach (List<string> lineData in ingredientCsvElementList)
        {
            if (lineData[0] == "") continue; // Skip empty lines

            IngredientValuesSo ingredientSoInstance;
            
            if (!_ingredients.Select(x => x.Name).Contains(lineData[0]))
            {
                ingredientSoInstance = ScriptableObject.CreateInstance<IngredientValuesSo>();
                
                string newIngredientDataPath = _ingredientDataFolderPath + "/D_" + lineData[0].Replace(" ", "") + "IngredientValues.asset";
                AssetDatabase.CreateAsset(ingredientSoInstance, newIngredientDataPath);
                
                _ingredients.Add(ingredientSoInstance);
            }
            else
            {
                ingredientSoInstance = _ingredients.Find(x => x.Name == lineData[0]);
            }
            
            List<WeatherStateSo> newWeatherStates = new();

            foreach (string weather in lineData[3].Split(", "))
            {
                WeatherStateSo weatherStateSoInstance = weatherStates.Find(x => x.Name == weather);
                
                if (weatherStateSoInstance)
                {
                    newWeatherStates.Add(weatherStateSoInstance);
                }
            }
            
            List<LunarCycleStateSo> newLunarCycleStates = new();
            
            foreach (string lunarCycle in lineData[4].Split(", "))
            {
                LunarCycleStateSo lunarCycleStateSoInstance = lunarCycleStates.Find(x => x.Name == lunarCycle);
                
                if (lunarCycleStateSoInstance)
                {
                    newLunarCycleStates.Add(lunarCycleStateSoInstance);
                }
            }
            
            Biome newBiomes = Biome.None;
            int biomeMaxValue = Enum.GetValues(typeof(Biome)).Cast<int>().Max();
            
            foreach (string newBiome in lineData[5].Split(", "))
            {
                string newBiomeText = newBiome.Replace(" ", "");
        
                for (int mask = biomeMaxValue; mask > 0; mask >>= 1)
                {
                    Biome currentBiome = (Biome)mask;
                    string biomeText = currentBiome.ToString();
            
                    if (biomeText == newBiomeText)
                    {
                        newBiomes |= currentBiome;
                        break; // Exit the loop once we find a match
                    }
                }
            }
            
            SpawnLocation newSpawnLocations = SpawnLocation.None;
            int spawnLocationMaxValue = Enum.GetValues(typeof(SpawnLocation)).Cast<int>().Max();
            
            foreach (string newSpawnLocation in lineData[6].Split(", "))
            {
                string newSpawnLocationText = newSpawnLocation.Replace(" ", "");
        
                for (int mask = spawnLocationMaxValue; mask > 0; mask >>= 1)
                {
                    SpawnLocation currentSpawnLocation = (SpawnLocation)mask;
                    string spawnLocationText = currentSpawnLocation.ToString();
            
                    if (spawnLocationText == newSpawnLocationText)
                    {
                        newSpawnLocations |= currentSpawnLocation;
                        break; // Exit the loop once we find a match
                    }
                }
            }
            
            ingredientSoInstance.SetData(lineData[0], lineData[2], newWeatherStates, newLunarCycleStates, newBiomes, newSpawnLocations);
            EditorUtility.SetDirty(ingredientSoInstance);
        }
        
        // ingredientListSo.IngredientValues.Clear();
        // ingredientListSo.IngredientValues.AddRange(_ingredients);
        // EditorUtility.SetDirty(ingredientListSo);
        
        // Recipes Csv
        csvFile = Resources.Load<TextAsset>(recipeDataCsv.name);
        GenerateCsvElementList(csvFile, out List<List<string>> recipeCsvElementList);
        
        foreach (List<string> lineData in recipeCsvElementList)
        {
            if (lineData[0] == "") continue; // Skip empty lines

            PotionValuesSo potionSoInstance;
            
            if (!_potions.Select(x => x.Name).Contains(lineData[0]))
            {
                potionSoInstance = ScriptableObject.CreateInstance<PotionValuesSo>();
                potionSoInstance.SetDefaultData(stirHapticChallengeSo, Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f),
                    Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f), Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f),
                    Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f), Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));
                
                string newPotionDataPath = _potionDataFolderPath + "/D_" + lineData[15] + lineData[0].Replace(" ", "") + "PotionValues.asset";
                AssetDatabase.CreateAsset(potionSoInstance, newPotionDataPath);
                
                _potions.Add(potionSoInstance);
            }
            else
            {
                potionSoInstance = _potions.Find(x => x.Name == lineData[0]);
            }
            
            PotionDifficultySo difficultySoInstance = _potionDifficulties.Find(x => x.Difficulty == int.Parse(lineData[15]));
            List<TemperatureChallengeIngredients> temperatureChallengeIngredients = new();
            int currentIngredientIndex = 2;
            for (int i = 10; i < 15; i++)
            {
                if (lineData[i] == "") continue;
                
                if (temperatureChallengeIngredients.Count == 0 || temperatureChallengeIngredients[^1].Temperature != 0)
                {
                    temperatureChallengeIngredients.Add(new TemperatureChallengeIngredients(new List<CookedIngredientForm>(), 0));
                }

                IngredientValuesSo ingredientSoInstance;

                switch (lineData[i])
                {
                    case "Add to Cauldron":
                        ingredientSoInstance = _ingredients.Find(x => x.Name == lineData[currentIngredientIndex + 1]);
                        for (int n = 0; n < int.Parse(lineData[currentIngredientIndex]); n++)
                        {
                            temperatureChallengeIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(ingredientSoInstance, null));
                        }
                        
                        currentIngredientIndex += 2;
                        break;
                    
                    case "Low Heat Bellows":
                        temperatureChallengeIngredients[^1] = new TemperatureChallengeIngredients(
                            temperatureChallengeIngredients[^1].CookedIngredients, (Temperature) 1);
                        break;
                    
                    case "Medium Heat Bellows":
                        temperatureChallengeIngredients[^1] = new TemperatureChallengeIngredients(
                            temperatureChallengeIngredients[^1].CookedIngredients, (Temperature) 2);
                        break;
                    
                    case "High Heat Bellows":
                        temperatureChallengeIngredients[^1] = new TemperatureChallengeIngredients(
                            temperatureChallengeIngredients[^1].CookedIngredients, (Temperature) 3);
                        break;
                    
                    case "Cut and Add to Cauldron":
                        ingredientSoInstance = _ingredients.Find(x => x.Name == lineData[currentIngredientIndex + 1]);
                        for (int n = 0; n < int.Parse(lineData[currentIngredientIndex]); n++)
                        {
                            temperatureChallengeIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(ingredientSoInstance, choppingHapticChallengeListSo));
                        }
                        
                        currentIngredientIndex += 2;
                        break;
                    
                    case "Grind and Add to Cauldron":
                        ingredientSoInstance = _ingredients.Find(x => x.Name == lineData[currentIngredientIndex + 1]);
                        for (int n = 0; n < int.Parse(lineData[currentIngredientIndex]); n++)
                        {
                            temperatureChallengeIngredients[^1].CookedIngredients.Add(new CookedIngredientForm(ingredientSoInstance, grindingHapticChallengeSo));
                        }
                        
                        currentIngredientIndex += 2;
                        break;
                }
            }

            potionSoInstance.SetData(lineData[0], lineData[1], difficultySoInstance, temperatureChallengeIngredients.ToArray());
            EditorUtility.SetDirty(potionSoInstance);
        }
        
        potionListSo.Potions.Clear();
        potionListSo.Potions.AddRange(_potions);
        EditorUtility.SetDirty(potionListSo);
        
        // Letters Csv
        csvFile = Resources.Load<TextAsset>(letterDataCsv.name);
        GenerateCsvElementList(csvFile, out List<List<string>> letterCsvElementList);

        for (int l = 0; l < letterCsvElementList.Count; l++)
        {
            List<string> lineData = letterCsvElementList[l];

            if (lineData[0] == "") continue; // Skip empty lines

            NarrativeBlockOfLettersContentSo narrativeBlock = gameDontDestroyOnLoadManager.AllNarrativeBlocksContentSo
                .Find(x => x.Name == lineData[0]);

            ClientSo clientSoInstance;

            if (!_clients.Select(x => x.Name).Contains(lineData[1]))
            {
                clientSoInstance = ScriptableObject.CreateInstance<ClientSo>();
                clientSoInstance.SetData(lineData[1], Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f));

                string newClientDataPath = _clientDataFolderPath + "/D_" + lineData[1].Replace(" ", "") + "Client.asset";
                AssetDatabase.CreateAsset(clientSoInstance, newClientDataPath);

                _clients.Add(clientSoInstance);
            }
            else
            {
                clientSoInstance = _clients.Find(x => x.Name == lineData[1]);
            }

            LetterContentSo letterContentSoInstance = ScriptableObject.CreateInstance<LetterContentSo>();
            
            narrativeBlock.Content.Add(letterContentSoInstance);
            EditorUtility.SetDirty(narrativeBlock);
            
            int moneyAmount = int.Parse(lineData[8]);

            if (lineData[7] != "")
            {
                bool canAdvanceQuestProgressionIndex = lineData[9] == "TRUE";
                int timeAfterSuccess = int.Parse(lineData[10]);

                LetterContentSo successLetterContentSoInstance = ScriptableObject.CreateInstance<LetterContentSo>();
                successLetterContentSoInstance.SetData(clientSoInstance, lineData[7]);

                string newSuccessLetterDataPath = _letterDataFolderPath + "/LS_" + lineData[1].Replace(" ", "") + "_" +
                                                  narrativeBlock.Content.Count.ToString("00") + ".asset";
                AssetDatabase.CreateAsset(successLetterContentSoInstance, newSuccessLetterDataPath);

                OrderContentSo orderContentSoInstance = ScriptableObject.CreateInstance<OrderContentSo>();
                List<PotionDemand> requestedPotions = new();

                for (int i = 2; i < 6; i++)
                {
                    if (lineData[i] == "") continue; // Skip empty fields

                    PotionValuesSo potionSoInstance = _potions.Find(x => x.Name == lineData[i]);

                    if (potionSoInstance)
                    {
                        requestedPotions.Add(new PotionDemand(potionSoInstance));
                    }
                }

                orderContentSoInstance.SetData(requestedPotions.ToArray(), moneyAmount);

                string newOrderDataPath = _orderDataFolderPath + "/O_" + lineData[1].Replace(" ", "") + "_" +
                                          narrativeBlock.Content.Count.ToString("00") + ".asset";
                AssetDatabase.CreateAsset(orderContentSoInstance, newOrderDataPath);

                letterContentSoInstance.SetData(clientSoInstance, lineData[6], orderContentSoInstance,
                    successLetterContentSoInstance, canAdvanceQuestProgressionIndex, timeAfterSuccess);
            }
            else
            {
                letterContentSoInstance.SetData(clientSoInstance, lineData[6], moneyAmount);
            }

            string newLetterDataPath = _letterDataFolderPath + "/L" + narrativeBlock.RequiredQuestProgressionIndex +
                                       "_" + lineData[1].Replace(" ", "") + "_" +
                                       (l == 0 ? "00" : narrativeBlock.Content.Count.ToString("00")) + ".asset";
            AssetDatabase.CreateAsset(letterContentSoInstance, newLetterDataPath);
        }
    }

    private static void GenerateCsvElementList(TextAsset csvFile, out List<List<string>> csvElementList)
    {
        string[] csvUnquotedStrings = csvFile.text.Split('"');
        bool isFirstElementEmpty = string.IsNullOrEmpty(csvUnquotedStrings[0]);
        csvUnquotedStrings = csvUnquotedStrings.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        csvElementList = new List<List<string>> { new() };
        for (int i = 0; i < csvUnquotedStrings.Length; i++)
        {
            if (i % 2 == (isFirstElementEmpty ? 1 : 0))
            {
                List<string> lines = csvUnquotedStrings[i].Split("\r\n").ToList();

                for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
                {
                    if (lineIndex != 0)
                    {
                        csvElementList.Add(new List<string>());
                    }

                    List<string> splitLine = lines[lineIndex].Split(",").ToList();
                    
                    if (i != 0 && lineIndex == 0) // If not the first line of the CSV
                    {
                        if (string.IsNullOrEmpty(splitLine[0]))
                        {
                            splitLine.RemoveAt(0); // Remove the first empty element
                        }
                    }
                    
                    if (i != csvUnquotedStrings.Length - 1 && lineIndex == lines.Count - 1) // If not the last line of the CSV
                    {
                        if (string.IsNullOrEmpty(splitLine[^1]))
                        {
                            splitLine.RemoveAt(splitLine.Count - 1); // Remove the last empty element
                        }
                    }
                    
                    csvElementList[^1].AddRange(splitLine);
                }
            }
            else
            {
                csvElementList[^1].Add(csvUnquotedStrings[i].Replace("\n\n", "\n"));
            }
        }
        
        csvElementList.RemoveAt(0); // Remove the first line which contains headers
    }
}
#endif
