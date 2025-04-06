using SombraStudios.Shared.Extensions;
using UnityEngine;

namespace LAGS
{
    public class IsometricSorting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Settings")]
        [Tooltip("Offset to add to the sorting order.")]
        [SerializeField] private int _sortingOrderOffset = 0;

        //private const float _factor = 0.01f;

        //private static Vector3 _origin;

        private void Awake()
        {
            this.SafeInit(ref _spriteRenderer);
        }

        private void LateUpdate()
        {
            // Lower values of (X + Y) mean objects should appear on top
            //float sortingValue = transform.position.x + transform.position.y; // Classic isometric sorting
            //float sortingValue = (transform.position.x + transform.position.y) * _factor; // Tunable bias toward one axis
            float sortingValue = transform.position.y; // Tunable bias toward one axis
            _spriteRenderer.sortingOrder = Mathf.RoundToInt((-sortingValue * 100) + _sortingOrderOffset);
        }
    }
}
