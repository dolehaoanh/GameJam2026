using AC.Attribute;
using AC.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AC.GameTool.SaveData
{
    public class SaveManager : Singleton<SaveManager>
    {
        private static readonly string saveFileName = "87c3e7e3a96f933b449d07a5a78f85af";
        private GameData _gameData;
        [SerializeField] IntGameEvent _coinChangeEvent;
        [ReadOnlly]
        public CheckLoadCompleted CheckLoadCompleted;
        protected override void Awake()
        {
            base.Awake();
            CheckLoadCompleted = new CheckLoadCompleted();
            LoadSaveData();
        }

        public GameData GameData => _gameData;

        public void SaveGameData()
        {
            SaveGameDataToFile(GameData);
        }

        public void LoadSaveData()
        {
            _gameData = LoadGameDataFromFile();
            if (!_gameData.IsGameDataCreated)
            {
                _gameData.IsGameDataCreated = true;
                _gameData.Coin = 0;
                _gameData.Music = true;
                _gameData.Sound = true;
                _gameData.Haptic = true;
                
                // Load fake leaderboard data on first launch
                LoadFakeLeaderboardData();
            }
            SaveGameData();
            CheckLoadCompleted.IsLoadCompleted = true;
        }
        
        /// <summary>
        /// Load fake leaderboard data (called on first game launch)
        /// </summary>
        private void LoadFakeLeaderboardData()
        {
            // Fake player names for Single Dive leaderboard
            List<string> singleDiveNames = new List<string>
            {
                "Captain Jack",
                "Marina Deep",
                "Coral Hunter",
                "Wave Rider",
                "Ocean Master"
            };
            
            // Fake player names for Total Coins leaderboard (different from Single Dive)
            List<string> totalCoinsNames = new List<string>
            {
                "Captain Jack",
                "Treasure King",
                "Golden Diver",
                "Ocean Master",
                "Coin Master"
            };
            
            // Store all fake player names
            _gameData.FakePlayerNames = new List<string>();
            _gameData.FakePlayerNames.AddRange(singleDiveNames);
            _gameData.FakePlayerNames.AddRange(totalCoinsNames);
            
            // Fake Top 5 Single Dive
            _gameData.Top5SingleDive = new List<int> { 3300, 2600, 2000, 1400, 720 };
            _gameData.Top5SingleDiveNames = new List<string>
            {
                singleDiveNames[0],
                singleDiveNames[1],
                singleDiveNames[2],
                singleDiveNames[3],
                singleDiveNames[4]
            };
            
            // Fake Top 5 Total Coins - using different names
            _gameData.Top5TotalCoins = new List<int> { 15000, 12500, 10800, 9200, 7500 };
            _gameData.Top5TotalCoinsNames = new List<string>
            {
                totalCoinsNames[0],
                totalCoinsNames[1],
                totalCoinsNames[2],
                totalCoinsNames[3],
                totalCoinsNames[4]
            };
        }
        
        /// <summary>
        /// Public method to reload fake data (for CheatUI)
        /// </summary>
        public void ReloadFakeLeaderboardData()
        {
            LoadFakeLeaderboardData();
            SaveGameData();
        }
        public void ClearSaveData()
        {
            DeleteSaveDataFile();
            _gameData = new GameData();
        }
        
        /// <summary>
        /// Clear all leaderboard data
        /// </summary>
        public void ClearLeaderboardData()
        {
            if (_gameData == null) return;
            
            _gameData.Top5SingleDive = new List<int>();
            _gameData.Top5TotalCoins = new List<int>();
            _gameData.BestSingleDiveCoin = 0;
            _gameData.TotalCoinsCollected = 0;
            SaveGameData();
            
            // Debug.Log("[SaveManager] Cleared all leaderboard data");
        }

        /// <summary>
        /// Set coin to an absolute value and raise event.
        /// </summary>
        public void SetCoin(int coin)
        {
            if (_gameData == null) return;
            int newValue = Mathf.Max(0, coin);
            _gameData.Coin = newValue;
            SaveGameData();
            if (_coinChangeEvent != null)
                _coinChangeEvent.RaiseEvent(newValue);
        }

        /// <summary>
        /// Add delta to coin (can be negative) and raise event.
        /// </summary>
        public void AddCoin(int delta)
        {
            if (_gameData == null) return;
            int newValue = Mathf.Max(0, _gameData.Coin + delta);
            _gameData.Coin = newValue;
            SaveGameData();
            if (_coinChangeEvent != null)
                _coinChangeEvent.RaiseEvent(newValue);
        }
        #region Read/Write File
        public static void SaveGameDataToFile(GameData gameData)
        {
            // Lưu dữ liệu vào tệp
            SaveToFile(gameData);
        }

        public static GameData LoadGameDataFromFile()
        {
            // Đọc dữ liệu từ tệp
            GameData gameData = LoadFromFile();
            return gameData;
        }



        private static void SaveToFile(GameData gameData)
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            try
            {
                // Sử dụng BinaryFormatter để giải mã dữ liệu từ dạng nhị phân
                BinaryFormatter formatter = new BinaryFormatter();
                // Lưu dữ liệu vào tệp
                using (FileStream fileStream = new FileStream(saveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    formatter.Serialize(fileStream, gameData);
                    fileStream.Close();
                }
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex);
            }           
        }

        private static GameData LoadFromFile()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            // Đọc dữ liệu từ tệp
            if (File.Exists(saveFilePath))
            {
                try
                {
                    // Sử dụng BinaryFormatter để giải mã dữ liệu từ dạng nhị phân
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read))
                    {
                        GameData gameData = formatter.Deserialize(fileStream) as GameData;
                        fileStream.Close();
                        return gameData;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                    return new GameData();
                }
            }
            else
            {
                Debug.LogWarning("Save file not found.");
                return new GameData();
            }
        }

        public static void DeleteSaveDataFile()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
        }

        /// <summary>
        /// Update leaderboard stats after completing a level
        /// </summary>
        /// <param name="finalCoins">Số coin thực tế nhận được (đã tính bonus/penalty)</param>
        public void UpdateLeaderboardStats(int finalCoins)
        {
            if (_gameData == null) return;
            if (finalCoins <= 0) return; // Không cập nhật nếu không có coin

            // Update best single dive (kỷ lục cá nhân) - dùng coin thực tế nhận được
            if (finalCoins > _gameData.BestSingleDiveCoin)
            {
                _gameData.BestSingleDiveCoin = finalCoins;
            }

            // Update total coins collected (tổng coin đã thu thập) - dùng coin thực tế nhận được
            _gameData.TotalCoinsCollected += finalCoins;
            
            // Update Top 5 Single Dive - so sánh với fake leaderboard
            UpdateTop5SingleDive(finalCoins);
            
            // Update Top 5 Total Coins - so sánh với fake leaderboard
            UpdateTop5TotalCoins(_gameData.TotalCoinsCollected);

            SaveGameData();
            
            // Debug.Log($"[SaveManager] Leaderboard updated - FinalCoins: {finalCoins}, BestSingleDive: {_gameData.BestSingleDiveCoin}, TotalCoins: {_gameData.TotalCoinsCollected}");
        }
        
        /// <summary>
        /// Update Top 5 Single Dive leaderboard
        /// Player's score competes with fake leaderboard scores
        /// Chỉ hiển thị kỷ lục cao nhất của người chơi (không duplicate)
        /// </summary>
        /// <param name="score">Số coin thực tế nhận được (đã tính bonus/penalty)</param>
        private void UpdateTop5SingleDive(int score)
        {
            if (_gameData.Top5SingleDive == null)
                _gameData.Top5SingleDive = new List<int>();
            if (_gameData.Top5SingleDiveNames == null)
                _gameData.Top5SingleDiveNames = new List<string>();
            
            string playerName = string.IsNullOrEmpty(_gameData.PlayerName) ? "Player" : _gameData.PlayerName;
            
            // Tìm vị trí của người chơi hiện tại trong leaderboard (nếu có)
            int existingIndex = -1;
            for (int i = 0; i < _gameData.Top5SingleDiveNames.Count; i++)
            {
                if (_gameData.Top5SingleDiveNames[i] == playerName)
                {
                    existingIndex = i;
                    break;
                }
            }
            
            // Nếu người chơi đã có trong leaderboard, chỉ update nếu điểm mới cao hơn
            if (existingIndex >= 0)
            {
                // Nếu điểm mới không cao hơn điểm hiện tại, không làm gì
                if (score <= _gameData.Top5SingleDive[existingIndex])
                    return;
                
                // Xóa entry cũ để thêm entry mới với điểm cao hơn
                _gameData.Top5SingleDive.RemoveAt(existingIndex);
                _gameData.Top5SingleDiveNames.RemoveAt(existingIndex);
            }
            
            // Kiểm tra xem score có đủ điều kiện vào top 5 không
            bool shouldAdd = _gameData.Top5SingleDive.Count < 5 || 
                             score > _gameData.Top5SingleDive[_gameData.Top5SingleDive.Count - 1];
            
            if (!shouldAdd) return;
            
            // Tìm vị trí chèn
            int insertIndex = _gameData.Top5SingleDive.Count;
            for (int i = 0; i < _gameData.Top5SingleDive.Count; i++)
            {
                if (score > _gameData.Top5SingleDive[i])
                {
                    insertIndex = i;
                    break;
                }
            }
            
            // Chèn score và tên người chơi
            _gameData.Top5SingleDive.Insert(insertIndex, score);
            _gameData.Top5SingleDiveNames.Insert(insertIndex, playerName);
            
            // Keep only top 5
            if (_gameData.Top5SingleDive.Count > 5)
            {
                _gameData.Top5SingleDive.RemoveRange(5, _gameData.Top5SingleDive.Count - 5);
                _gameData.Top5SingleDiveNames.RemoveRange(5, _gameData.Top5SingleDiveNames.Count - 5);
            }
        }
        
        /// <summary>
        /// Update Top 5 Total Coins leaderboard
        /// Player's total coins competes with fake leaderboard scores
        /// Chỉ update nếu total coins mới cao hơn và chưa có trong list
        /// </summary>
        private void UpdateTop5TotalCoins(int totalCoins)
        {
            if (_gameData.Top5TotalCoins == null)
                _gameData.Top5TotalCoins = new List<int>();
            if (_gameData.Top5TotalCoinsNames == null)
                _gameData.Top5TotalCoinsNames = new List<string>();
            
            // Kiểm tra xem totalCoins có đủ điều kiện vào top 5 không
            bool shouldAdd = _gameData.Top5TotalCoins.Count < 5 || 
                             totalCoins > _gameData.Top5TotalCoins[_gameData.Top5TotalCoins.Count - 1];
            
            if (!shouldAdd) return;
            
            // Tìm vị trí của người chơi hiện tại trong leaderboard (nếu có)
            string playerName = string.IsNullOrEmpty(_gameData.PlayerName) ? "Player" : _gameData.PlayerName;
            int existingIndex = -1;
            for (int i = 0; i < _gameData.Top5TotalCoinsNames.Count; i++)
            {
                if (_gameData.Top5TotalCoinsNames[i] == playerName)
                {
                    existingIndex = i;
                    break;
                }
            }
            
            // Nếu người chơi đã có trong leaderboard, xóa entry cũ
            if (existingIndex >= 0)
            {
                _gameData.Top5TotalCoins.RemoveAt(existingIndex);
                _gameData.Top5TotalCoinsNames.RemoveAt(existingIndex);
            }
            
            // Tìm vị trí chèn
            int insertIndex = _gameData.Top5TotalCoins.Count;
            for (int i = 0; i < _gameData.Top5TotalCoins.Count; i++)
            {
                if (totalCoins > _gameData.Top5TotalCoins[i])
                {
                    insertIndex = i;
                    break;
                }
            }
            
            // Chèn totalCoins và tên người chơi
            _gameData.Top5TotalCoins.Insert(insertIndex, totalCoins);
            _gameData.Top5TotalCoinsNames.Insert(insertIndex, playerName);
            
            // Keep only top 5
            if (_gameData.Top5TotalCoins.Count > 5)
            {
                _gameData.Top5TotalCoins.RemoveRange(5, _gameData.Top5TotalCoins.Count - 5);
                _gameData.Top5TotalCoinsNames.RemoveRange(5, _gameData.Top5TotalCoinsNames.Count - 5);
            }
        }
        
        /// <summary>
        /// Manually set Top 5 Single Dive (for editor/testing)
        /// </summary>
        public void SetTop5SingleDive(List<int> top5)
        {
            if (_gameData == null) return;
            _gameData.Top5SingleDive = new List<int>(top5);
            _gameData.Top5SingleDive.Sort((a, b) => b.CompareTo(a));
            if (_gameData.Top5SingleDive.Count > 5)
                _gameData.Top5SingleDive.RemoveRange(5, _gameData.Top5SingleDive.Count - 5);
            SaveGameData();
        }
        
        /// <summary>
        /// Manually set Top 5 Total Coins (for editor/testing)
        /// </summary>
        public void SetTop5TotalCoins(List<int> top5)
        {
            if (_gameData == null) return;
            _gameData.Top5TotalCoins = new List<int>(top5);
            _gameData.Top5TotalCoins.Sort((a, b) => b.CompareTo(a));
            if (_gameData.Top5TotalCoins.Count > 5)
                _gameData.Top5TotalCoins.RemoveRange(5, _gameData.Top5TotalCoins.Count - 5);
            SaveGameData();
        }
        #endregion
    }
}


