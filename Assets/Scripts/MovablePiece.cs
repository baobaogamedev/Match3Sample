using System.Collections;
using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(GamePiece))]
    public class MovablePiece : MonoBehaviour
    {
        private GamePiece _piece;
        private Coroutine _moveCoroutine;

        private void Awake()
        {
            _piece = GetComponent<GamePiece>();
        }

        public void Move(int newX, int newY, float duration)
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = StartCoroutine(MoveCoroutine(newX, newY, duration));
        }

        private IEnumerator MoveCoroutine(int newX, int newY, float duration)
        {
            _piece.X = newX;
            _piece.Y = newY;

            Vector3 startPos = transform.position;
            Vector3 endPos = _piece.GridControllerRef.GetWorldPosition(newX, newY);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
        }
    }
}
