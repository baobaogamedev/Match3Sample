using System.Collections;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(GamePiece))]
    public class RemovablePiece : MonoBehaviour
    {
        [Tooltip("Animation clip to play when the piece is cleared.")]
        public AnimationClip clearAnimation;

        public bool IsBeingCleared { get; private set; }

        protected GamePiece piece;
        private Animator animator;

        private void Awake()
        {
            piece = GetComponent<GamePiece>();
            animator = GetComponent<Animator>();

            if (clearAnimation == null)
            {
                Debug.LogWarning("Clear animation is not assigned to RemovablePiece on " + gameObject.name);
            }
        }

        public virtual void Clear()
        {
            if (IsBeingCleared)
                return;

            IsBeingCleared = true;
            piece.GridControllerRef.level.OnPieceCleared(piece);
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            if (animator != null && clearAnimation != null)
            {
                animator.Play(clearAnimation.name);
                yield return new WaitForSeconds(clearAnimation.length);
            }
            else
            {
                Debug.LogWarning("Missing animator or clearAnimation on " + gameObject.name);
            }

            Destroy(gameObject);
        }
    }
}
