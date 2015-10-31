using UnityEngine;
using System.Collections;

public class ProjectileCollider : MonoBehaviour 
{
	private GameObject explosion;

	private float rayonExplosion, forceExplosion, forceSoulevante;

	// Use this for initialization
	void Start () 
	{
		explosion = Resources.Load ("Fireworks") as GameObject;
	}

	public void setExplosion (float forceExplosion, float rayonExplosion, float forceSoulevante)
	{
		this.rayonExplosion = rayonExplosion;
		this.forceExplosion = forceExplosion;
		this.forceSoulevante = forceSoulevante;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		string rootName = collision.transform.root.name;
		if (rootName == "Cars") 
		{
			// Position finale de la carapace :
			Vector3 positionFinale = transform.position;

			// Détruire la carapace :
			Destroy (gameObject);

			// Effet visuel de l'explosion : 
			GameObject fx = Instantiate (explosion) as GameObject;
			fx.transform.position = positionFinale;
			Destroy (fx, 1);

			// Ajouter une force d'explosion aux voitures proches :
			Collider[] collidersProches = Physics.OverlapSphere(positionFinale, rayonExplosion);
			for (int i = 0; i < collidersProches.Length; i++) 
			{
				Transform objetProche = collidersProches[i].transform;
				if (objetProche.root.name == "Cars") 
				{
					string nomVoitureProche = objetProche.parent.parent.name;
					if (nomVoitureProche != "Cars") 
					{
						Rigidbody rb = objetProche.parent.parent.GetComponent<Rigidbody>();
						if (rb  != null) {
							rb.AddExplosionForce(forceExplosion, positionFinale, rayonExplosion, forceSoulevante, ForceMode.Impulse);
							Debug.Log("Explosion sur " + nomVoitureProche);
						}
					}
				}
			}
		}
	}
}
