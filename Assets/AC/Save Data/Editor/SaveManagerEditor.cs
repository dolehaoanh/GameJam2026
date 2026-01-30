using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AC.GameTool.SaveData
{
    public class SaveManagerEditor : EditorWindow
    {
        [SerializeField] GameData _gameData;
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private PropertyField _gameDataField;
        private Button _btnLoad, _btnSave, _btnDelete;
        [MenuItem("Game Tool/Save Manager")]
        public static void ShowWinDow()
        {
            SaveManagerEditor wnd = GetWindow<SaveManagerEditor>();
            wnd.minSize = new Vector2(800, 600);
            wnd.maxSize = wnd.minSize;
            wnd.titleContent = new GUIContent("Save Manager");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
            LoadAllElement(root);

            LoadSaveData();
        }

        void LoadAllElement(VisualElement root)
        {
            _gameDataField = root.Q<PropertyField>("GameDataField");
            _btnLoad = root.Q<Button>("BtnLoadData");
            _btnSave = root.Q<Button>("BtnSaveData");
            _btnDelete = root.Q<Button>("BtnDeleteData");
            BindingAndAddRegisterElement();
        }

        void BindingAndAddRegisterElement()
        {
            SerializedObject so = new SerializedObject(this);
            var dataProperty = so.FindProperty("_gameData");
            _gameDataField.Bind(so);
            _btnLoad.clickable.clicked += BtnLoadClick;
            _btnSave.clickable.clicked += BtnSaveClick;
            _btnDelete.clickable.clicked += BtnDeletedClick;
        }

        private void BtnLoadClick()
        {
            LoadSaveData();
            Debug.Log("Load Game Data");
        }
        private void BtnSaveClick()
        {
            SaveManager.SaveGameDataToFile(_gameData);
            Debug.Log("Save Game Data");
        }


        public void BtnDeletedClick()
        {
            SaveManager.DeleteSaveDataFile();
            _gameData = new GameData();
            PlayerPrefs.DeleteAll();
            Debug.Log("Clear Game Data");
        }
        void LoadSaveData()
        {
            var saveDataTmp = SaveManager.LoadGameDataFromFile();
            if (saveDataTmp != null)
            {
                _gameData = saveDataTmp;
            }
            else
            {
                _gameData = new GameData();
            }
        }
    }
}

