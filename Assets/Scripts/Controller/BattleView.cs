using System.Linq;
using Scripts.Touches;
using UnityEngine;


namespace Scripts.Controller{
	public class BattleView : MonoBehaviour
	{
		private const float ROTATE_DEGREE_BY_MM = 90f/60f;
		[SerializeField] BattleController _battleController;
		[SerializeField] private TouchController touchController;

		private BattleTouchHandler _battleTouchHandler = new();
		private UnitController _selectedUnit;
		private Vector3 _offsetFromUnit;
		private float zDistance;

		// Use this for initialization
		void Awake () {
			Init();
		}
		

		private void Init()
		{
			touchController.AddHandler(_battleTouchHandler);
			_battleTouchHandler.OnTouchMoveEvent += OnTouchMove;
			_battleTouchHandler.OnTrabslateByZEvent += TranslateByZ;
			_battleTouchHandler.OnTouchStartEvent += OnToucStart;
		}

		private void OnToucStart(Vector3 screenPos)
		{
			if (_battleController.IsBattleRunning)
			{
				_selectedUnit = null;
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(screenPos);
			var hits = Physics.RaycastAll(ray);
			
			_selectedUnit = hits.Select(it => it.collider.gameObject.GetComponent<UnitController>())
				.FirstOrDefault( it => it != null);
			if (_selectedUnit != null)
			{
				zDistance = Camera.main.WorldToScreenPoint(_selectedUnit.transform.position).z;
				screenPos.z = zDistance;
				var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
				_offsetFromUnit = worldPos - _selectedUnit.transform.position;
			}
		}
		private void OnTouchMove(Vector3 screenPos, Vector3 screenMove)
		{
			if (_selectedUnit != null && screenMove != Vector3.zero)
			{
				screenPos.z = zDistance;
				var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
				_selectedUnit.transform.position = worldPos - _offsetFromUnit;
				return;
			}

			var moveInMM = 25.4f * screenMove.magnitude / Screen.dpi;
			if (Mathf.Abs(screenMove.y) > Mathf.Abs(screenMove.x))
			{
				var xRotateDirection = screenMove.y > 0 ? 1 : -1;
				var xAngle = xRotateDirection * moveInMM * ROTATE_DEGREE_BY_MM;
				transform.RotateAround(transform.position, Vector3.right, xAngle);
			}
			else
			{
				var yRotateDirection = screenMove.x > 0 ? -1 : 1;
				var yAngle = yRotateDirection * moveInMM * ROTATE_DEGREE_BY_MM;
				transform.RotateAround(transform.position, Vector3.up, yAngle);
			}
		}

		private void TranslateByZ(float delta)
		{
			var btlTransform = gameObject.transform;
			var deltaPos = btlTransform.position - Camera.main.transform.position;
			var factor = delta > 0 ? - 0.2f : 0.2f;
			btlTransform.position = btlTransform.position + deltaPos*factor;
		}
	}
}