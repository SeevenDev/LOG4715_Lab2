using UnityEngine;
using System.Collections;
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

	private int _currentMaxLap = 0; // garder la trace du tour le plus avancé
	private Transform[] _path_v;

	// Use this for initialization
	void Awake () 
	{
		foreach (CarController car in _carContainer.GetComponentsInChildren<CarController>(true))
		{
			_carPositions[car] = new PositionData();
		}

		// Chargement du path pour les voitures :
		GameObject p = GameObject.Find ("Path V");
		this._path_v = new Transform[p.transform.childCount];
		for (int i = 0; i < p.transform.childCount; i++) {
			this._path_v[i] = p.transform.GetChild(i);
		}
	}

	void Update()
	{
		// Méca 5 (rubberbanding) : Mise à jour de la vitesse maxdes voitures selon leur position :
		StartCoroutine (updateCarsSpeed ());
	}

	IEnumerator updateCarsSpeed()
	{
		yield return new WaitForSeconds (1.0f);

		CarController[] cars = getCarsInOrder ();
		for (int i = 0 ; i < cars.Length ; i++)
		{
			Vector3 velo = cars[i].rigidbody.velocity;
			float facteur = Mathf.Log(i+10, 10); // Premier = 1 : pas de changement
			cars[i].setMaxSpeedFactor(facteur);
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

					// Mettre à jour le tour le plus avancé :
					if (carData.lap > _currentMaxLap) {
						_currentMaxLap = carData.lap;
					}

					if (IsPlayer(car))
					{
						GetComponent<RaceManager>().Announce("Tour " + (carData.lap + 1).ToString());

						// Mec1. Style :
						car.GetComponent<StyleManager>().logStyle(100, "Tour complet");
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

	public CarController getCarAtPosition(int position, List<string> ignore)
	{
		return getCarsInOrder(ignore)[position];
	}

	public CarController[] getCarsInOrder()
	{
		return getCarsInOrder (new List<string> ());
	}

	public CarController[] getCarsInOrder(List<string> ignore)
	{
		Dictionary<CarController,float[]> cars = new Dictionary<CarController,float[]>();

		// === Calculer la spécificité des voitures ===

		float max_distWpProchain = 0;
		foreach (KeyValuePair<CarController,PositionData> pair in _carPositions)
		{
			// Ignorer les joueurs que l'on ne veut pas classer
			// (typiquement "Joueur 1" et/ou "Joueur 2") :
			if (ignore.IndexOf(pair.Key.transform.name) != -1) {
				continue;
			}

			// [0] : tour actuel (entre 0 et 2)
			// [1] : Waypoint le plus proche (entre 0 et 15)
			// [2] : distance au waypoint prochain (entre 0 et +Infinity ?)
			// [3] : spécificité convertie en nombre (partie Tri)
			float[] specificite = new float[4];

			// --- [0] Tour actuel ---

			specificite[0] = pair.Value.lap;

			// --- [1] Waypoint le plus proche ---

			float minDistWp = Mathf.Infinity;

			// Pour chaque checkpoint du Path Vehicule :
			for (int wp = 0 ; wp < _path_v.Length; wp++)
			{
				// On détermine vers quel waypoint la voiture est la plus proche :
				float distWp_tmp = (pair.Key.transform.position - _path_v[wp].transform.position).magnitude;
				
				if (distWp_tmp < minDistWp) {
					minDistWp = distWp_tmp;
					specificite[1] = wp;
				}
			}

			// --- [2] Distance au waypoint + 1 ---

			int wp_prochain = ((int)specificite[1] + 1) % _path_v.Length;
			specificite[2] = (pair.Key.transform.position - _path_v[wp_prochain].transform.position).magnitude;

			// On stock la valeur de la plus grande distance pour avoir l'ordre de grandeur pour le tri :
			if (specificite[2] > max_distWpProchain) {
				max_distWpProchain = specificite[2];
			}

			// --- Stockage de la spécificité dans le Dictionary ---

			cars.Add (pair.Key, specificite);
		}

		// === Trier les voitures selon leur spécificité ===

		// --- Convertir la spécificité en un chiffre ---

		float ordreGrandeurDist = Mathf.Pow(10, Mathf.Floor (Mathf.Log (max_distWpProchain, 10)));
		foreach (KeyValuePair<CarController,float[]> pair in cars) 
		{
			pair.Value[3] = pair.Value[2]/ordreGrandeurDist
				+ pair.Value[1]*10
					+ pair.Value[0]*100;
		}

		// --- Tri du dictionnaire en fonction de la spécificité ---

		List<KeyValuePair<CarController,float[]>> cars_list = new List<KeyValuePair<CarController,float[]>>(cars);
		cars_list.Sort (delegate(KeyValuePair<CarController,float[]> firstPair, KeyValuePair<CarController,float[]> nextPair) {
			return -1 * firstPair.Value[3].CompareTo (nextPair.Value[3]);
		});

		// --- On retourne un tableau de voitures triées selon leur position sur la piste ---

		List<CarController> voituresOrdonnees = new List<CarController>();
		foreach (KeyValuePair<CarController,float[]> pair in cars_list) {
			voituresOrdonnees.Add(pair.Key);
		}

		return voituresOrdonnees.ToArray();
	}

	bool IsPlayer(CarController car)
	{
		return car.GetComponent<CarUserControlMP>() != null;
	}
}
