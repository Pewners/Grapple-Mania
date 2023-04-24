using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor
{
	// basic settings
	public float castLength = 1f;

	// starting point
	private Vector3 origin = Vector3.zero;

	public enum CastDirection
	{
		Forward,
		Right,
		Up,
		Backward,
		Left,
		Down
	}
	private CastDirection castDirection;

	// stored hit information
	private bool hasDetectedHit;
	private Vector3 hitPosition;
	private Vector3 hitNormal;
	private float hitDistance;
	private List<Collider> hitColliders = new List<Collider>();
	private List<Transform> hitTransforms = new List<Transform>();

	// references
	private Transform tr;
	private Collider col;

	// layers
	public LayerMask layermask = 255;
	int ignoreRaycastLayer;

	// optional list of colliders to ignore
	private Collider[] ignoreList;
	private int[] ignoreListLayers;

	// debugging
	public bool isInDebugMode = false;


	// constructor
	public Sensor(Transform _transform, Collider _collider)
	{
		tr = _transform;

		if (_collider == null)
			return;

		ignoreList = new Collider[1];

		// add collider to ignore list;
		ignoreList[0] = _collider;

		// store "Ignore Raycast" layer number for later;
		ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

		// setup array to store ignore list layers;
		ignoreListLayers = new int[ignoreList.Length];
	}

	// reset variables that store information
	private void ResetFlags()
	{
		hasDetectedHit = false;
		hitPosition = Vector3.zero;
		hitNormal = -GetCastDirection();
		hitDistance = 0f;

		if (hitColliders.Count > 0)
			hitColliders.Clear();
		if (hitTransforms.Count > 0)
			hitTransforms.Clear();
	}

	private void GetStartPosition()
    {
		origin = Vector3.zero;
		/// this function could be extended to handle arrays of raycasts
    }

	// function to start a cast, called every frame by mover script;
	public void Cast()
	{
		ResetFlags();

		// origin and direction
		Vector3 _worldDirection = GetCastDirection();
		Vector3 _worldOrigin = tr.position;

		// check if ignore list length has been changed since last frame;
		if (ignoreListLayers.Length != ignoreList.Length)
		{
			// if so, setup ignore layer array to fit new length;
			ignoreListLayers = new int[ignoreList.Length];
		}

		// (Temporarily) move all objects in ignore list to 'Ignore Raycast' layer;
		for (int i = 0; i < ignoreList.Length; i++)
		{
			ignoreListLayers[i] = ignoreList[i].gameObject.layer;
			ignoreList[i].gameObject.layer = ignoreRaycastLayer;
		}

		// start raycast
		CastRay(_worldOrigin, _worldDirection);

		// reset collider layers in ignoreList;
		for (int i = 0; i < ignoreList.Length; i++)
		{
			ignoreList[i].gameObject.layer = ignoreListLayers[i];
		}
	}

	// cast a single ray into '_direction' from '_origin';
	private void CastRay(Vector3 _origin, Vector3 _direction)
	{
		RaycastHit _hit;
		hasDetectedHit = Physics.Raycast(_origin, _direction, out _hit, castLength, layermask, QueryTriggerInteraction.Ignore);

		// store information
		if (hasDetectedHit)
		{
			hitPosition = _hit.point;
			hitNormal = _hit.normal;

			hitColliders.Add(_hit.collider);
			hitTransforms.Add(_hit.transform);

			hitDistance = _hit.distance;
		}
	}

	// calculate a direction in world coordinates
	Vector3 GetCastDirection()
	{
		switch (castDirection)
		{
			case CastDirection.Forward:
				return tr.forward;

			case CastDirection.Right:
				return tr.right;

			case CastDirection.Up:
				return tr.up;

			case CastDirection.Backward:
				return -tr.forward;

			case CastDirection.Left:
				return -tr.right;

			case CastDirection.Down:
				return -tr.up;
			default:
				return Vector3.one;
		}
	}

	#region Getters

	public bool HasDetectedHit()
	{
		return hasDetectedHit;
	}

	public float GetDistance()
	{
		return hitDistance;
	}

	public Vector3 GetNormal()
	{
		return hitNormal;
	}

	public Vector3 GetPosition()
	{
		return hitPosition;
	}

	// returns a reference to the collider that was hit by the raycast;
	public Collider GetCollider()
	{
		return hitColliders[0];
	}

	// returns a reference to the transform component attached to the collider that was hit by the raycast;
	public Transform GetTransform()
	{
		return hitTransforms[0];
	}

	#endregion

	#region Setters

	// set the position for the raycast to start from;
	// the input vector '_origin' is converted to local coordinates;
	public void SetCastOrigin(Vector3 _origin)
	{
		if (tr == null) return;
		origin = tr.InverseTransformPoint(_origin);
	}

	// set which axis of this gameobject's transform will be used as the direction for the raycast;
	public void SetCastDirection(CastDirection _direction)
	{
		if (tr == null) return;
		castDirection = _direction;
	}

	public void SetCastLength(float _castLength)
	{
		castLength = _castLength;
	}

	#endregion

	#region Debugging

	// draw debug information in editor (hit positions and ground surface normals);
	public void DrawDebug()
	{
		if (hasDetectedHit && isInDebugMode)
		{
			Debug.DrawRay(hitPosition, hitNormal, Color.red, Time.deltaTime);
			float _markerSize = 0.2f;
			Debug.DrawLine(hitPosition + Vector3.up * _markerSize, hitPosition - Vector3.up * _markerSize, Color.green, Time.deltaTime);
			Debug.DrawLine(hitPosition + Vector3.right * _markerSize, hitPosition - Vector3.right * _markerSize, Color.green, Time.deltaTime);
			Debug.DrawLine(hitPosition + Vector3.forward * _markerSize, hitPosition - Vector3.forward * _markerSize, Color.green, Time.deltaTime);
		}
	}

    #endregion
}
