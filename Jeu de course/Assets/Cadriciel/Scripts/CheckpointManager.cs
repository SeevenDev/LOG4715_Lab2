using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour 
{

	[SerializeField]
	private GameObject _carContainer;

	[SerializeField]
	private int _checkPointCount;
	[SerializeField]
	private int _totalLaps;

	private bool _finished = false;
	
	private Dictionary<CarController,PositionData> _carPositions = new Dictionary<CarController, PositionData>();

	private class PositionData
	{
		public int lap;
		public int checkPoint;
		public int position;
	}

	private int _currentMaxLap = 0; // garder la trace du tour le plus avancé (pour 

	// Use this for initialization
	void Awake () 
	{
		foreach (CarController car in _carContainer.GetComponentsInChildren<CarController>(true))
		{
			_carPositions[car] = new PositionData();
		}
	}

	public void CheckpointTriggered(CarController car, int checkPointIndex)
	{

		PositionData carData = _carPositions[car];

		if (!_finished)
		{
			if (checkPointIndex == 0)
			{
				if (carData.checkPoint == _checkPointCount-1)
				{
					carData.checkPoint = checkPointIndex;
					carData.lap += 1;
					Debug.Log(car.name + " lap " + carData.lap);

					// Mettre à jour le tour le plus avancé :
					if (carData.lap > _currentMaxLap) {
						_currentMaxLap = carData.lap;
					}

					if (IsPlayer(car))
					{
						GetComponent<RaceManager>().Announce("Tour " + (carData.lap+1).ToString());

						// Mec1. Style :
						car.GetComponent<StyleManager>().logStyle(100, "Tour " + carData.lap+1);
					}

					if (carData.lap >= _totalLaps)
					{
						_finished = true;
						GetComponent<RaceManager>().EndRace(car.name.ToLower());

						if (IsPlayer(car))
						{
							// Mec1. Style :
							car.GetComponent<StyleManager>().logStyle(1000, "Premier !");
						}
					}
				}
			}
			else if (carData.checkPoint == checkPointIndex-1) //Checkpoints must be hit in order
			{
				carData.checkPoint = checkPointIndex;
			}
		}


	}

	/**
	 * Retourne les voitures qui sontau tour le plus avancé de la course.
	 **/
	public Transform[] getFirstCars()
	{
		List<Transform> firstCars_list = new List<Transform>();

		// Stocker toutes les voitures qui sont au tour le plus avancé :
		foreach (KeyValuePair<CarController,PositionData> pair in _carPositions)
		{
			if (pair.Value.lap == _currentMaxLap) {
				firstCars_list.Add(pair.Key.transform);
			}
		}

		return firstCars_list.ToArray();
	}

	bool IsPlayer(CarController car)
	{
		return car.GetComponent<CarUserControlMP>() != null;
	}
}
