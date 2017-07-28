using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
/*
	contains the list of all recipes in the game indexed via <string, Recipe> dictionary
*/
[XmlRoot("Recipes")]
public class Recipes{
	
	//list of recipes
	[XmlArray("_recipes")]
	[XmlArrayItem("Recipe")]
	public List<Recipe> recipes;
	
	//figure out how to load this from start
	//public static Dictionary<tuple<string, string>, Recipe> recipeList;
	public static Dictionary<int, Recipe> recipeList2;
	public static Dictionary<Ingredients, Recipe> recipeList3;
	public static Dictionary<int, Recipe> recipeList4;
	/*
	void Awake()
	{
		//recipeList = Load(path);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}*/
	
	public void Test()
	{
		recipeList3 = new Dictionary<Ingredients, Recipe>();
		
		Ingredients AB = new Ingredients(0, "A", "B");
		Ingredients BA = new Ingredients(1, "B", "A");
		Ingredients CA = new Ingredients(2, "C", "A");
		
		Recipe C = new Recipe("C", "A", "B");
		recipeList3.Add(AB, C);
		
		Recipe C2 = Cook(BA);
		Recipe C3 = Cook(CA);
	}

	
	//load the recipeList
	public static /*Dictionary<Ingredients, Recipe>*/Recipes Load(string Path, ItemDatabase DB)
	{
		if(DB.initialized == false)
			DB.initialize();
		
		TextAsset _xml = Resources.Load<TextAsset>(Path);
		XmlDocument xmldoc = new XmlDocument();
		xmldoc.LoadXml(_xml.text);
		
		XmlSerializer serial = new XmlSerializer(typeof(Recipes));
		StringReader reader = new StringReader(_xml.text);
		
		Recipes tmp = (Recipes) serial.Deserialize(reader);
		
		reader.Close();
		//parse through the recipes, adding them to the hash table
		
		recipeList3 = new Dictionary<Ingredients, Recipe>();
		recipeList4 = new Dictionary<int, Recipe>();
		
		string i1;
		string i2;
		int ID;
		int ID1;
		int ID2;
		
		for( int i = 0; i < tmp.recipes.Count ; i++)
		{
			//create an ingredient for each recipe's ingredients and add that to
			//recipeList3
			Recipe R = tmp.recipes[i];
			//Debug.Log(R._name.Replace("\t",""));
			//Debug.Log(R._item1.Replace("\t",""));
			i1 = R._item1.Replace("\t","");
			i2 = R._item2.Replace("\t","");
			i1 = i1.Replace("\n","");
			i2 = i2.Replace("\n","");
			
			Ingredients In = new Ingredients(i, i1, i2);
			recipeList3.Add(In, R);
			
			//Debug.Log(R._name.Replace("\t", "") + " " + i1+ " " + i2);
			
			if(DB.ExistingItemBank.ContainsKey(i1) && DB.ExistingItemBank.ContainsKey(i2))
			{
				Debug.Log("Bingo! It was " + i1 + " and " + i2);
				ID1 = DB.ExistingItemBank[i1].ID;
				ID2 = DB.ExistingItemBank[i2].ID;
			}
			else
			{
				ID1 = -1;
				ID2 = 0;
				Debug.Log("damn you " + i1 + " " + i2);
			}
			
			if(ID1<ID2)
				ID = Pair(ID1, ID2);
			else
				ID = Pair(ID2, ID1);
			
			//Debug.Log(R._name.Replace("\t", "") + " " + i1+ " " + i2 + " " + ID);
			
			if(ID1!=-1)
				recipeList4.Add(ID, R);
			
		}
		
		return tmp;
	}
	
	//pairing function, currently not in use
	public static int Pair(int a, int b)
	{
		return (1/2 * (a+b) * (a+b+1)) + b;
	}
	
	//return the recipe, since we don't have references to the game objects?
	//or maybe we do, who knows
	public static Recipe Cook(Ingredients i)
	{
		if(recipeList3.ContainsKey(i))
		{
			Debug.Log("Found the item!");
			return recipeList3[i];
		}
		Debug.Log("Couldn't Find!");
		return null;
	}
	
	public static Recipe Cook(int i)
	{
		if(recipeList4.ContainsKey(i))
		{
			Debug.Log("Found the item!");
			return recipeList4[i];
		}
		Debug.Log("Couldn't Find!");
		return null;
	}
	
	public static Recipe Cook(int a, int b)
	{
		int ID;
		
		if(a<b)
			ID = Pair(a, b);
		else
			ID = Pair(b, a);
		
		Debug.Log(ID);
		
		if(recipeList4.ContainsKey(ID))
		{
			Debug.Log("Found the item!");
			return recipeList4[ID];
		}
		Debug.Log("Couldn't Find!");
		return null;
	}
	
	//return a game object for the corresponding items via the hash table
	public static GameObject Cook(Item i1, Item i2)
	{
		//use pairing function as key instead
		//&(a, b) = 1/2(a+b)(a+b+1) + b
		//Infinite hotel construction?
		//Cantor pairing
		//2^a*3^b, use different primes as bases
		
		int ID = Pair(i1.ID, i2.ID);
		
		if(recipeList2.ContainsKey(ID))
			return /*recipeList2[ID];*/null;
		
		//Tuple ingredients = new Tuple<string, string>(i1, i2);
		//if(recipeList.ContainsKey(ingredients))
			///*return recipeList[ingredients];*/return null;
		return null;
	}
}
