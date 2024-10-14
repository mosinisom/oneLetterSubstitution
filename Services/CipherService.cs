using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services
{
  public class CipherService
  {
    private static readonly string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    private static readonly Random random = new Random();
    private static readonly Dictionary<char, double> RussianLetterFrequencies = new Dictionary<char, double>
    {
      {'о', 0.10983}, {'е', 0.08483}, {'а', 0.07998}, {'и', 0.07367}, {'н', 0.06700},
      {'т', 0.06318}, {'с', 0.05473}, {'р', 0.04746}, {'в', 0.04533}, {'л', 0.04343},
      {'к', 0.03486}, {'м', 0.03203}, {'д', 0.02977}, {'п', 0.02804}, {'у', 0.02615},
      {'я', 0.02001}, {'ы', 0.01898}, {'ь', 0.01735}, {'г', 0.01687}, {'з', 0.01641},
      {'б', 0.01592}, {'ч', 0.01450}, {'й', 0.01208}, {'х', 0.00966}, {'ж', 0.00940},
      {'ш', 0.00718}, {'ю', 0.00639}, {'ц', 0.00486}, {'щ', 0.00361}, {'э', 0.00331},
      {'ф', 0.00267}, {'ъ', 0.00037}, {'ё', 0.00013}
    };

    public string Encrypt(string text, string key)
    {
      var keyMap = CreateKeyMap(Alphabet, key);
      return new string(text.Select(c => keyMap.ContainsKey(c) ? keyMap[c] : c).ToArray());
    }

    public string Decrypt(string text, string key)
    {
      var keyMap = CreateKeyMap(Alphabet, key);
      var reverseKeyMap = CreateReverseKeyMap(keyMap);
      return new string(text.Select(c => reverseKeyMap.ContainsKey(c) ? reverseKeyMap[c] : c).ToArray());
    }

    public string GenerateKey()
    {
      var shuffledAlphabet = Alphabet.OrderBy(_ => random.Next()).ToArray();
      return new string(shuffledAlphabet);
    }

    public string HackKey(string text)
    {
      var textFrequencies = CalculateFrequencies(text);
      var sortedTextFrequencies = textFrequencies.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
      var sortedRussianFrequencies = RussianLetterFrequencies.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

      var hackMap = new Dictionary<char, char>();

      for (int i = 0; i < sortedTextFrequencies.Count; i++)
      {
        hackMap[sortedTextFrequencies[i]] = sortedRussianFrequencies[i];
      }

      // Создание ключа на основе карты замены
      var hackedKey = new char[Alphabet.Length];
      var usedChars = new HashSet<char>(hackMap.Values);

      for (int i = 0; i < Alphabet.Length; i++)
      {
        if (hackMap.ContainsKey(Alphabet[i]))
        {
          hackedKey[i] = hackMap[Alphabet[i]];
        }
        else
        {
          // Найти первый неиспользованный символ
          hackedKey[i] = Alphabet.First(c => !usedChars.Contains(c));
          usedChars.Add(hackedKey[i]);
        }
      }

      return new string(hackedKey);
    }

    private Dictionary<char, char> CreateKeyMap(string alphabet, string key)
    {
      if (key.Distinct().Count() != key.Length)
      {
        throw new ArgumentException("Key contains duplicate characters.");
      }

      var keyMap = new Dictionary<char, char>();
      for (int i = 0; i < alphabet.Length; i++)
      {
        keyMap[alphabet[i]] = key[i];
      }
      return keyMap;
    }

    private Dictionary<char, char> CreateReverseKeyMap(Dictionary<char, char> keyMap)
    {
      var reverseKeyMap = new Dictionary<char, char>();
      foreach (var kvp in keyMap)
      {
        if (reverseKeyMap.ContainsKey(kvp.Value))
        {
          throw new ArgumentException($"Duplicate value found in key map: {kvp.Value}");
        }
        reverseKeyMap[kvp.Value] = kvp.Key;
      }
      return reverseKeyMap;
    }

    private Dictionary<char, double> CalculateFrequencies(string text)
    {
      var frequencies = new Dictionary<char, double>();
      int totalLetters = 0;

      foreach (var c in text)
      {
        if (Alphabet.Contains(c))
        {
          if (!frequencies.ContainsKey(c))
          {
            frequencies[c] = 0;
          }
          frequencies[c]++;
          totalLetters++;
        }
      }

      foreach (var key in frequencies.Keys.ToList())
      {
        frequencies[key] = Math.Round(frequencies[key] / totalLetters, 8);
      }

      return frequencies;
    }
  }
}