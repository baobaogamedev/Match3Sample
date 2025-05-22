using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class LevelSelect : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonPlayerPrefs
        {
            public GameObject gameObject;
            public string playerPrefKey;
        }

        [Header("Button Configuration")]
        public ButtonPlayerPrefs[] buttons;

        private void Start()
        {
            foreach (var button in buttons)
            {
                int score = PlayerPrefs.GetInt(button.playerPrefKey, 0);
                Transform parentTransform = button.gameObject.transform;

                for (int i = 1; i <= 3; i++)
                {
                    Transform star = parentTransform.Find($"star{i}");
                    if (star != null)
                    {
                        star.gameObject.SetActive(i <= score);
                    }
                    else
                    {
                        Debug.LogWarning($"Missing star{i} in {button.gameObject.name}");
                    }
                }
            }
        }

        public void OnButtonPress(string levelName)
        {
            if (!string.IsNullOrEmpty(levelName))
            {
                SceneManager.LoadScene(levelName);
            }
            else
            {
                Debug.LogError("Level name is null or empty.");
            }
        }
    }
}
