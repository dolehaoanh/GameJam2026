using System.Collections.Generic;

namespace AC.GameTool.SaveData
{
    [System.Serializable]
    public class GameData
    {
        public bool IsGameDataCreated;
        public int Level;
        public int Coin;
        public bool Sound;
        public bool Music;
        public bool Haptic;
        public bool HasSeenStory;
        public bool IsTutorialCompleted;
        public bool HasSeenDepthZoneTutorial;
        
        // Inventory Data
        public List<string> UnlockedItemNames = new List<string>();
        public Dictionary<int, string> EquippedItemsBySlot = new Dictionary<int, string>();

        // Item upgrade levels, keyed by item asset name
        public Dictionary<string, int> ItemLevels = new Dictionary<string, int>();

        // Leaderboard Data
        public string PlayerName = ""; // Tên người chơi
        public bool HasSetPlayerName = false; // Đã đặt tên chưa
        
        public int BestSingleDiveCoin = 0;  // Số coin thu thập nhiều nhất trong 1 lần lặn
        public int TotalCoinsCollected = 0; // Tổng số coin đã thu thập được
        
        // Top 5 Leaderboards (sorted descending)
        public List<int> Top5SingleDive = new List<int>(); // Top 5 best single dive scores
        public List<string> Top5SingleDiveNames = new List<string>(); // Names for Top 5 single dive
        
        public List<int> Top5TotalCoins = new List<int>(); // Top 5 total coins milestones
        public List<string> Top5TotalCoinsNames = new List<string>(); // Names for Top 5 total coins
        
        // Fake player names for initial leaderboard
        public List<string> FakePlayerNames = new List<string>();
    }
}
