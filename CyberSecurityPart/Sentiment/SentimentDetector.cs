namespace CyberSecurityAwarenessBot.Sentiment
{
    public enum SentimentType { Neutral, Worried, Frustrated, Curious, Happy, Confused }

    public class SentimentResult
    {
        public SentimentType Type { get; init; } = SentimentType.Neutral;
        public string Label { get; init; } = "Neutral";
    }

    /// <summary>
    /// Detects the user's emotional tone from keyword patterns
    /// so the bot can respond with empathy and the right tone.
    /// </summary>
    public class SentimentDetector
    {
        private static readonly string[] Worried = { "worried", "scared", "afraid", "nervous", "anxious", "fear", "hacked", "unsafe", "vulnerable", "danger", "panicking", "oh no", "help me" };
        private static readonly string[] Frustrated = { "frustrated", "annoyed", "angry", "upset", "useless", "terrible", "hate", "impossible", "so hard", "ugh", "fed up", "i give up", "ridiculous" };
        private static readonly string[] Curious = { "curious", "interested", "want to know", "wondering", "how does", "what is", "explain", "tell me about", "how do", "why is", "i want to learn" };
        private static readonly string[] Happy = { "great", "awesome", "amazing", "love it", "thank you", "thanks", "helpful", "brilliant", "excellent", "fantastic", "glad", "perfect", "cheers" };
        private static readonly string[] Confused = { "confused", "don't understand", "not sure", "what do you mean", "unclear", "lost", "don't get it", "huh", "i don't follow", "explain again" };

        public SentimentResult Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new();
            string l = input.ToLower();
            if (Hit(l, Worried)) return new() { Type = SentimentType.Worried, Label = "Worried" };
            if (Hit(l, Frustrated)) return new() { Type = SentimentType.Frustrated, Label = "Frustrated" };
            if (Hit(l, Confused)) return new() { Type = SentimentType.Confused, Label = "Confused" };
            if (Hit(l, Curious)) return new() { Type = SentimentType.Curious, Label = "Curious" };
            if (Hit(l, Happy)) return new() { Type = SentimentType.Happy, Label = "Happy" };
            return new();
        }

        private static bool Hit(string input, string[] words) => words.Any(input.Contains);
    }
}
