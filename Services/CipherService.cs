using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services
{
  public class CipherService
  {
    private static readonly string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    private static readonly Random random = new Random();

    public string Encrypt(string text, string key)
    {
      var keyMap = CreateKeyMap(Alphabet, key);
      return new string(text.Select(c => keyMap.ContainsKey(c) ? keyMap[c] : c).ToArray());
    }

    public string Decrypt(string text, string key)
    {
      var keyMap = CreateKeyMap(Alphabet, key);
      var reverseKeyMap = keyMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
      return new string(text.Select(c => reverseKeyMap.ContainsKey(c) ? reverseKeyMap[c] : c).ToArray());
    }

    public string GenerateKey()
    {
      var shuffledAlphabet = Alphabet.OrderBy(_ => random.Next()).ToArray();
      return new string(shuffledAlphabet);
    }

    public string HackCipher(string text)
    {
      // TODO Implement hack cipher

      return "Взлом невозможен в демонстрационной версии";
    }

    private Dictionary<char, char> CreateKeyMap(string alphabet, string key)
    {
      var keyMap = new Dictionary<char, char>();
      for (int i = 0; i < alphabet.Length; i++)
      {
        keyMap[alphabet[i]] = key[i];
      }
      return keyMap;
    }
  }
}
