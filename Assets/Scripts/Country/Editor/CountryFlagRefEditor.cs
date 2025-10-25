using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using DefaultNamespace.Country;

[CustomEditor(typeof(CountryFlagRef))]
public class CountryFlagRefEditor : Editor
{
	private CountryFlagRef flagRef;

	private void OnEnable()
	{
		flagRef = (CountryFlagRef)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (GUILayout.Button("Load Flags from Folder"))
		{
			string folderPath = EditorUtility.OpenFolderPanel("Select Flag Folder", "Assets", "");
			if (!string.IsNullOrEmpty(folderPath))
			{
				LoadFlags(folderPath);
			}
		}
	}

	private void LoadFlags(string folderPath)
	{
		// Convert absolute path to relative Unity path
		string relativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);

		// Find all sprites in folder
		string[] assetPaths = AssetDatabase.FindAssets("t:Sprite", new[] { relativePath })
			.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			.ToArray();

		// Clear current dictionary
		flagRef.codeToImg.Clear();

		foreach (string path in assetPaths)
		{
			Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
			if (sprite != null)
			{
				string key = Path.GetFileNameWithoutExtension(path); // filename as key
				if (!flagRef.codeToImg.ContainsKey(key))
				{
					flagRef.codeToImg.Add(key, sprite);
				}
			}
		}

		EditorUtility.SetDirty(flagRef);
		AssetDatabase.SaveAssets();

		Debug.Log($"Loaded {flagRef.codeToImg.Count} flags into {flagRef.name}");
	}
}