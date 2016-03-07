using UnityEngine;
using System.Collections.Generic;

public class Creator : MonoBehaviour {

	private Vector3 size;
	private List<GameObject> objects;

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

	public List<string> types = new List<string> () {"tree", "rock", "plant", "air"};
	public List<GameObject> treelist;
	public List<GameObject> rocklist;
	public List<GameObject> plantlist;

	// Use this for initialization
	void Start () {
		objects = new List<GameObject> ();
		this.size = GameObject.Find ("Plane").GetComponent<Collider> ().bounds.size;
		print(size);
		treelist = new List<GameObject>() { treefab1, treefab2, treefab3, treefab4, treefab5 };
		plantlist = new List<GameObject>() { bushfab1, bushfab2, bushfab3, bushfab4 };
		rocklist = new List<GameObject>() { rockfab1, rockfab2, rockfab3 };
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
//	void MakeBoundary() {
//		bool r = true;
//		for (int i=(int)size.x; i > ((size.x+25)*-1); i-=25) {
//			GameObject prefab = r ? rockWallPrefab1 : rockWallPrefab2;
//			int h = r ? 5 : 1;
//			GameObject rock = (GameObject)GameObject.Instantiate(prefab, new Vector3(i, 0, ((size.x)*-1)), Quaternion.identity);
//			rock.transform.localScale = new Vector3(2, h, 2);
//			GameObject rock2 = (GameObject)GameObject.Instantiate(prefab, new Vector3(i, 0, (size.x)), Quaternion.identity);
//			rock2.transform.localScale = new Vector3(2, h, 2);
//			GameObject rock3 = (GameObject)GameObject.Instantiate(prefab, new Vector3((size.x), 0, i), Quaternion.identity);
//			rock3.transform.localScale = new Vector3(2, h, 2);
//			GameObject rock4 = (GameObject)GameObject.Instantiate(prefab, new Vector3(((size.x)*-1), 0, i), Quaternion.identity);
//			rock4.transform.localScale = new Vector3(2, h, 2);
//			r = !r;
//		}
//	}

	//Makes a forest
	void MakeForest() {

		float threshhold = 2;
		int chunksize = 40;
		int imax = (int)size.x / chunksize;
		int jmax = (int)size.z / chunksize;
		Dictionary<string, Attribute> seed = new Dictionary<string, Attribute> () { 
			{"tree", new Attribute (Random.Range(0f, 1f), Random.Range(0.8f, 3f), Random.Range(0f, 1f), threshhold) },
			{"plant", new Attribute (Random.Range(0f, 1f), Random.Range(0.8f, 3f), Random.Range(0f, 1f), threshhold) },
			{"rock", new Attribute (Random.Range(0f, 1f), Random.Range(0.8f, 3f), Random.Range(0f, 1f), threshhold) },
			{"air", new Attribute (Random.Range(0f, 1f), Random.Range(0.8f, 3f), Random.Range(0f, 1f), threshhold) }
		};

		Chunk[,] chunks = new Chunk[imax, jmax];

		for (int i = 0; i<imax; i++) { 
			Chunk[] subchunks = new Chunk [jmax];

			for (int j = 0; j < jmax; j++) {
				Dictionary<string, Attribute> variation = new Dictionary<string, Attribute>();
				if (i == 0 && j == 0) {
					variation = seed;
				} else {
					Chunk lChunk;
					Chunk tChunk;
					if (i == 0) {
						tChunk = lChunk = subchunks [j - 1];
					}
					else if (j == 0) {
						tChunk = lChunk = chunks[i - 1, j];
					} 
					else {	
						tChunk = chunks [i - 1, j];
						lChunk = subchunks [j - 1];
					}

					foreach (var type in types) {
						Attribute att = new Attribute().getNext(lChunk.variation[type], tChunk.variation[type]);
						variation.Add (type, att);
					}
				}
				subchunks[j] = new Chunk (new Vector2(i * chunksize, j* chunksize), chunksize, variation, this);
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
		//fobjects = rmFObjInArea ();

		foreach (FObject fobject in fobjects) {
			GameObject gentree = (GameObject)GameObject.Instantiate (fobject.prefab, fobject.position, Quaternion.identity);
			gentree.transform.localScale = fobject.scale;
			objects.Add (gentree);
		}
	}

	public List<FObject> rmFObjInArea(List<FObject> fobjects, Vector3 start, Vector3 end) {
		foreach (FObject fobject in fobjects) {
			
		}
		return fobjects;
	}

	public class Attribute {
		public float probability;
		public float scale;
		public float variation;
		public float t;

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

		public int startpos;
		public int size;
		public List<FObject> fobjects;
		public Dictionary<string, Attribute> variation;

		public Chunk(Vector2 startpos, int size, Dictionary<string, Attribute> variation, Creator c) {
			this.variation = variation;

			fobjects = new List<FObject>();

			for ( int i = (int) startpos.x; i < (int)startpos.x + size;) {
				int maxi = i;
				for (int j = (int) startpos.y; j < (int)startpos.y + size;) {

					//random roll for iterm class
					//todo probability
					//random roll for item type
					int itype = Random.Range(0, variation.Count);
					string type = c.types [itype];

					float temp = 0;
					GameObject prefab = c.treefab1;

					switch (type) {
					case "air":
						j += (int)(10 * Random.Range(1f, 2.5f));
						continue;
					case "tree":
						prefab = c.treelist[Random.Range(0, c.treelist.Count)];
						temp = 3 * variation[type].scale;
						break;
					case "rock":
						prefab = c.rocklist[Random.Range(0, c.rocklist.Count)];
						temp = 20 * variation[type].scale;
						break;
					case "plant":
						prefab = c.plantlist[Random.Range(0, c.plantlist.Count)];
						temp = 2 * variation[type].scale;
						break;
					}

					//TODO Scale is still iffy
					fobjects.Add(new FObject(prefab, new Vector3(i, 0, j), new Vector3(1, variation[type].scale, 1)));

					j += (int)temp;
					if ((int)temp + i > maxi) {
						maxi = (int)temp + i;
					}
				}
				i = Random.Range(i, maxi);
			}
		}

		public List<FObject> getObjects() {
			return this.fobjects;
		}
	}

	//TODO introduce subclasses for object types
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
