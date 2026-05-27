namespace CyberSecurityAwarenessBot.Memory
{
    /// <summary>
    /// Remembers the user's name, favourite topic, and message count
    /// so the bot can personalise responses throughout the session.
    /// </summary>
    public class UserMemory
    {
        public string Name { get; private set; } = "there";
        public string? FavouriteTopic { get; private set; }
        public int MessageCount { get; private set; } = 0;

        private readonly List<string> _history = new();

        public void SetName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = char.ToUpper(name.Trim()[0]) + name.Trim()[1..].ToLower();
        }

        public void SetFavouriteTopic(string topic)
        {
            FavouriteTopic = topic;
            if (!_history.Contains(topic)) _history.Add(topic);
        }

        public void IncrementMessageCount() => MessageCount++;

        /// <summary>Returns a memory recall hint after several messages.</summary>
        public string GetMemoryRecall()
        {
            if (FavouriteTopic != null && MessageCount > 3)
                return $"As someone interested in {FavouriteTopic}, you might find this especially relevant.";
            return string.Empty;
        }

        public IReadOnlyList<string> History => _history.AsReadOnly();
    }
}
