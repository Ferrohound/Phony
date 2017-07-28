using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeLoader : MonoBehaviour {
	
	public string path;
	public ItemDatabase DB;

	// Use this for initialization
	void Start () {
		if(Recipes.recipeList3 == null)
			Recipes.Load(path, DB);
	}
}
