using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class NotesBuilder
    {
        private readonly List<string> _entries = new List<string>();

        public void Add(string note)
        {
            if (!string.IsNullOrWhiteSpace(note))
            {
                _entries.Add(note);
            }
        }

        public void AddRange(string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
            {
                return;
            }

            string[] parts = notes.Split(';');
            foreach (var part in parts)
            {
                Add(part.Trim());
            }
        }

        public override string ToString()
        {
            return string.Join("; ", _entries);
        }
    }
}