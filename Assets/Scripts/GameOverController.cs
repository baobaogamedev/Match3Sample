using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Match3
{
    public class GameOverController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject screenParent;
        [SerializeField] private GameObject scoreParent;
        [SerializeField] private Text loseText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Image[] stars;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            screenParent.SetActive(false);
            foreach (var star in stars)
            {
                star.enabled = false;
            }
        }

        public void ShowLose()
        {
            screenParent.SetActive(true);
            scoreParent.SetActive(false);

            if (animator)
            {
                animator.Play("GameOverShow");
            }
        }

        public void ShowWin(int score, int starCount)
        {
            screenParent.SetActive(true);
            loseText.enabled = false;
            scoreText.text = score.ToString();
            scoreText.enabled = false;

            if (animator)
            {
                animator.Play("GameOverShow");
            }

            StartCoroutine(ShowWinCoroutine(starCount));
        }

        private IEnumerator ShowWinCoroutine(int starCount)
        {
            yield return new WaitForSeconds(0.5f);

            starCount = Mathf.Clamp(starCount, 0, stars.Length);

            for (int i = 0; i < starCount; i++)
            {
                stars[i].enabled = true;
                yield return new WaitForSeconds(0.5f);
            }

            scoreText.enabled = true;
        }

        public void OnReplayClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnDoneClicked()
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }
}
