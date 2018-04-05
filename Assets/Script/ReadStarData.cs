using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadStarData : MonoBehaviour {


    public string filename = "starDataForUnity2";

    public int numberOfStars;

	ParticleSystem pSystem;
	ParticleSystem.Particle[] particles; 
	int numberOfParticles = 0;

	public int GetNumberOfStars(){
		return numberOfStars;
	}

	public bool printData;

	List<Dictionary<string,object>> data;

	void Awake() {
		
		data = CSVReader.Read (filename);

		if (printData) {
			//for(var i=0; i < data.Count; i++) {
			for (var i=0; i < 20; i++) {
				print ("X " + data [i] ["X"] + " " +
					"Y " + data [i] ["Y"] + " " +
					"Z " + data [i] ["Z"] + " " +
					"S " + data [i] ["S"]);
			}
		}


		numberOfStars = data.Count;


	}


	void Start () {

		makeLattice ();

	
	}

	void Update () {




	}

	public float positionScale = 1;
	public float sizeScale = 1;
	
	int particlesBuffer;

	private void makeLattice (){


		////PARTICLE SYSTEM SETUP
		pSystem = GetComponent<ParticleSystem> ();


		pSystem.maxParticles = numberOfStars;

		pSystem.SetParticles (particles, numberOfStars);

		pSystem.startColor = Color.green;

		particles = new ParticleSystem.Particle[pSystem.maxParticles]; 
		

		///// ASSIGNING TO PARTICLE ARRAY

		//get particles buffer
		particlesBuffer =  pSystem.GetParticles(particles);


		//loop through stars, assigning position colour and brightness (size)
		for (int i = 0; i < numberOfStars; i++) {

			Vector3 position = 0.1f * new Vector3 (float.Parse ( data [i] ["X"].ToString()),
			                           				float.Parse ( data [i] ["Y"].ToString()),
			                           				float.Parse ( data [i] ["Z"].ToString()));

			float size = float.Parse ( data [i] ["S"].ToString())/5f;

			if(size >1){
				size=1;
			}




			Color colour = new Color ( float.Parse ( data [i] ["R"].ToString()),
			                           float.Parse ( data [i] ["G"].ToString()),
			                           float.Parse ( data [i] ["B"].ToString()),size);
			                    



			particles [i].position = position * positionScale;
											
			particles [i].color = colour;

			//particles [i].color.a = 0;//Mathf.Log(size);
							
			particles [i].size = 1f * sizeScale;

			//Debug.Log(size);

						
		}

		//apply positions, colours etc to particles

		pSystem.SetParticles(particles, numberOfStars);

		

		

		
	}





}
