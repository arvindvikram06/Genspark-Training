using System;
using System.Collections.Generic;
using WordGame.Interfaces;


namespace WordGame.Repositories
{
    public class WordRepository : IWordRepository
    {
        private readonly List<string> _words = new List<string>
        {
            "APPLE", "MANGO", "GRAPE", "TRAIN", "PLANT", "BRAIN"
        };

        public string GetRandomWord()
        {
           return _words[Random.Shared.Next(_words.Count)];
        }
    }
}