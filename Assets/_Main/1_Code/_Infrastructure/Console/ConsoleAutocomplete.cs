using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using VContainer;

public class ConsoleAutocomplete : MonoBehaviour
{
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text suggestionsText; // Один компонент текста под консолью

        private GameData _gameData;

        [Inject]
        public void Construct(GameData gameData)
        {
            _gameData = gameData;
        }

        public void Init()
        {
            inputField.onValueChanged.AddListener(OnInputTextChanged);
            suggestionsText.text = string.Empty;
        }

        public void CleanUp()
        {
            inputField.onValueChanged.RemoveListener(OnInputTextChanged);
            suggestionsText.text = string.Empty;
        }

        private void OnInputTextChanged(string currentText)
        {
            // Если в строке пусто или игрок уже нажал пробел (вводит значение) — скрываем подсказки
            if (string.IsNullOrEmpty(currentText) || currentText.Contains(" "))
            {
                suggestionsText.text = string.Empty;
                return;
            }

            string searchPattern = currentText.ToLower();

            // Находим все параметры, которые начинаются с того, что ввёл игрок
            List<string> matches = _gameData.GetAllParamNames()
                .Where(paramName => paramName.StartsWith(searchPattern))
                .ToList();

            // Если совпадений нет — очищаем текст
            if (matches.Count == 0)
            {
                suggestionsText.text = string.Empty;
                return;
            }

            // Собираем все совпадения в одну строку через двойной пробел для читаемости
            StringBuilder sb = new StringBuilder();
            foreach (string match in matches)
            {
                sb.AppendLine(match);
            }

            suggestionsText.text = sb.ToString();
        }
    }
