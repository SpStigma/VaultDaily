using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using VaultDaily.Models;

namespace VaultDaily.Services
{
    public class JournalService
    {
        private readonly string _storageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Mon journal");

        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public JournalService()
        {
            if(!Directory.Exists(_storageFolder))
            {
                Directory.CreateDirectory(_storageFolder);
            }
        }

        private string GetFilePath(DateTime date)
        {
            return Path.Combine(_storageFolder, $"{date:yyyy-MM-dd}.json");
        }

        public void SaveEntry(Journal entry)
        {
            string filePath = GetFilePath(entry.Date);

            string jsonString =
                JsonSerializer.Serialize(
                    entry,
                    _jsonOptions);

            File.WriteAllText(filePath, jsonString);
        }

        public Journal LoadEntry(DateTime date)
        {
            string filePath = GetFilePath(date);

            if(File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Journal>(jsonString, _jsonOptions) ?? new Journal { Date = date };
            }
            return new Journal { Date = date };
        }

        public List<DateTime> GetAllJournalDates()
        {
            var dates = new List<DateTime>();
            if (Directory.Exists(_storageFolder))
            {
                var files = Directory.GetFiles(_storageFolder, "*.json");
                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (DateTime.TryParse(fileName, out DateTime date))
                    {
                        dates.Add(date);
                    }
                }
            }
            // Trie pour avoir les plus récents en premier
            return dates.OrderByDescending(d => d).ToList();
        }
    }
}
