using UnityEngine;
using System.Collections.Generic;

public class Creator : MonoBehaviour {

	public int size = 500; //World size

	public GameObject rockWallPrefab1;
	public GameObject rockWallPrefab2;
	public GameObject treefab1;
	public GameObject treefab2;
	public GameObject treefab3;
	public GameObject treefab4;
	public GameObject treefab5;
	public GameObject bushfab1;
	public GameObject bushfab2;
	public GameObject bushfab3;
	public GameObject bushfab4;
	public GameObject rockfab1;
	public GameObject rockfab2;
	public GameObject rockfab3;
	private List<GameObject> objects;

	/*public List<GameObject> go = new List<GameObject>() {
		rockWallPrefab1,
		rockWallPrefab2,
		treefab1,
		treefab2,
		treefab3,
		treefab4,
		treefab4,
		treefab5,
		bushfab1,
		bushfab2,
		bushfab3,
		bushfab4,
		rockfab1,
		rockfab2,
		rockfab3
	};

	public List<GameObject> getGO() {
		return this.go;
	}*/

	// Use this for initialization
	void Start () {
		objects = new List<GameObject> ();
		//MakeRockWall();
		MakeForest();
	}

	void OnDestroy() {
		foreach (GameObject g in objects) {
			Destroy (g);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Makes a rock wall
	void MakeRockWall() {
		bool r = true;
		for (int i=(size); i>((size+25)*-1); i-=25) {
			GameObject prefab = r ? rockWallPrefab1 : rockWallPrefab2;
			int h = r ? 5 : 1;
			GameObject rock = (GameObject)GameObject.Instantiate(prefab, new Vector3(i, 0, ((size)*-1)), Quaternion.identity);
			rock.transform.localScale = new Vector3(2, h, 2);
			GameObject rock2 = (GameObject)GameObject.Instantiate(prefab, new Vector3(i, 0, (size)), Quaternion.identity);
			rock2.transform.localScale = new Vector3(2, h, 2);
			GameObject rock3 = (GameObject)GameObject.Instantiate(prefab, new Vector3((size), 0, i), Quaternion.identity);
			rock3.transform.localScale = new Vector3(2, h, 2);
			GameObject rock4 = (GameObject)GameObject.Instantiate(prefab, new Vector3(((size)*-1), 0, i), Quaternion.identity);
			rock4.transform.localScale = new Vector3(2, h, 2);
			r = !r;
		}
	}

	//Makes a forest
	void MakeForest() {

		List<string> types = new List<string> () {"tree", "rock", "plant", "air"};

		float threshhold = 2;
		int chunksize = 10;
		int imax = ((size - 30) * 2 ) / chunksize;
		int jmax = ((size - 30) * 2 ) / chunksize;

		Chunk[,] chunks = new Chunk[imax, jmax];

		for (int i = 0; i<imax; i++) { 
			Chunk[] subchunks = new Chunk [jmax];

			for (int j = 0; j < jmax; j++) {
				Chunk chunk;
				if (i == 0 ) {
					if (j == 0) {
						Dictionary<string, Attribute> variation = new Dictionary<string, Attribute>();
						foreach (var type in types) {
							Attribute att = new Attribute (Random.Range(0f, 1f), Random.Range(0.8f, 3f), Random.Range(0f, 1f), threshhold);
							variation.Add (type, att);
						}
						chunk = new Chunk(new Vector2(i * size, j* size), size, variation, this);
					}
					else {
						Dictionary<string, Attribute> variation = new Dictionary<string, Attribute>();
						Chunk lChunk = subchunks [j - 1];
						Chunk tChunk = lChunk;
						foreach (var type in types) {
							Attribute att = new Attribute().getNext(lChunk.variation[type], tChunk.variation[type]);
							variation.Add (type, att);
						}
						chunk = new Chunk(new Vector2(i * size, j* size), size, variation, this);
					}
				}
				else {
					if (j == 0) {
						Dictionary<string, Attribute> variation = new Dictionary<string, Attribute>();
						Chunk tChunk = chunks[i - 1, j];
						Chunk lChunk = tChunk;
						foreach (var type in types) {
							Attribute att = new Attribute().getNext(lChunk.variation[type], tChunk.variation[type]);
							variation.Add (type, att);
						}
						chunk = new Chunk(new Vector2(i * size, j* size), size, variation, this);
					} else {	
						Dictionary<string, Attribute> variation = new Dictionary<string, Attribute> ();
						Chunk lChunk = subchunks [j - 1];
						Chunk tChunk = chunks [i - 1, j];
						foreach (var type in types) {
							Attribute att = new Attribute ().getNext (lChunk.variation [type], tChunk.variation [type]);
							variation.Add (type, att);
						}
						chunk = new Chunk (new Vector2(i * size, j* size), size, variation, this);
					}
				}
				subchunks[j] = chunk;
			}
			for (int z = 0; z < subchunks.Length; z++) {
				chunks [i, z] = subchunks[z];
			}
		}

		//todo carve out path (stretch)

		//instantiate gameobject
		List<FObject> fobjects = new List<FObject>();
		for (int i = 0; i < imax; i++) {
			for (int j = 0; j < jmax; j++) {
				foreach (FObject fobject in chunks[i,j].getObjects()) {
					fobjects.Add (fobject);
				}
			}
		}

		//delete
		//fobjects = removeFObject ();

		foreach (FObject fobject in fobjects) {
			GameObject gentree = (GameObject)GameObject.Instantiate (fobject.prefab, fobject.position, Quaternion.identity);
			gentree.transform.localScale = fobject.scale;
			objects.Add (gentree);
		}
	}

	public List<FObject> removeFObject(List<FObject> fobjects, Vector3 start, Vector3 end) {
		foreach (FObject fobject in fobjects) {
			
		}
		return fobjects;
	}

	public class Attribute {
		public float t;
		public float probability;
		public float scale;
		public float variation;

		public Attribute() {}

		public Attribute(float probability, float scale, float variation, float threshhold) {
			this.probability = probability;
			this.scale = scale;
			this.variation = variation;
			this.t = threshhold;
		}

		public Attribute getNext(Attribute a, Attribute b) {

			float av;

			av = (a.probability + b.probability) / 2;
			float newProb = Random.Range(av-t, av+t);

			av = (a.scale + b.scale) / 2;
			float newScale = Random.Range(av-t, av+t);

			av = (a.variation + b.variation) / 2;
			float newVar = Random.Range(av-t, av+t);

			return new Attribute (newProb, newScale, newVar, t);
		}
	}

	public class Chunk {

		//public List<GameObject> go = getGO();

		public int startpos;
		public int size;
		public List<FObject> fobjects;
		public Dictionary<string, Attribute> variation;

		public Chunk(Vector2 startpos, int size, Dictionary<string, Attribute> variation, Creator c) {
			this.variation = variation;

			string[] types = new string[]{"tree", "rock", "plant", "air"};
			fobjects = new List<FObject>();

			for ( int i = (int) startpos.x; i < (int)startpos.x + size;) {
				int maxi = i;
				for (int j = (int) startpos.y; j < (int)startpos.y + size;) {
					//random roll for iterm class
					//todo probability
					//random roll for item type
					int itype = Random.Range(0, variation.Count);
					string type = types [itype];

					int temp = 0;
					GameObject prefab = c.treefab1;
					int subtype;

					switch (type) {
					case "air":
						j += (int)(10 * Random.Range(1f, 2.5f));
						continue;
					case "tree":
						subtype = Random.Range (0, 5);
						switch (subtype) {
						case 1:
							prefab = c.treefab1;
							break;
						case 2:
							prefab = c.treefab2;
							break;
						case 3:
							prefab = c.treefab3;
							break;
						case 4:
							prefab = c.treefab4;
							break;
						case 5:
							prefab = c.treefab5;
							break;
						}
						temp = (int)(3);
						break;
					case "plant":
						subtype = Random.Range (0, 4);
						switch (subtype) {
						case 1:
							prefab = c.bushfab1;
							break;
						case 2:
							prefab = c.bushfab2;
							break;
						case 3:
							prefab = c.bushfab3;
							break;
						case 4:
							prefab = c.bushfab4;
							break;
						}
						temp = (int)(2);
						break;
					case "rock":
							subtype = Random.Range (0, 3);
							switch (subtype) {
							case 1:
								prefab = c.rockfab1;
								break;
							case 2:
								prefab = c.rockfab2;
								break;
							case 3:
								prefab = c.rockfab3;
								break;
							}
						temp = (int)(20);
						break;
					}

					j += temp;
					//Vector3 v = new Vector3 (i - 150, 0, j-150) + Random.Range(-10f,10f) * new Vector3(1/2.5f,0,1);
					Vector3 v = new Vector3 (i, 0 , j) + Random.Range(-5f, 5f) * new Vector3(-1, 0, 1);
					float _size = Random.Range (0.5f, 1.5f);
					FObject fobject = new FObject(prefab, v, new Vector3 (_size, variation[type].scale, _size));
					fobjects.Add (fobject);

					if (temp + i > maxi) {
						maxi = temp + i;
					}
				}
				i = maxi;//+= maxi;
			}
		}

		public List<FObject> getObjects() {
			return this.fobjects;
		}
	}

	public class FObject {
		public GameObject prefab;
		public Vector3 position;
		public Vector3 scale;

		public FObject(GameObject prefab, Vector3 position, Vector3 scale) {
			this.prefab = prefab;
			this.position = position;
			this.scale = scale;
		}
	}
}
