
using System;
using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.Networking;

public class StreamedTexture
{
	public string textureUrl = string.Empty;

	private Texture2D texture = null;

	public bool requestPending = false;
	public bool requestCompleted = false;

	public void ChangeTexture(string url)
	{
		textureUrl = url;

		requestPending = true;
	}

	public Texture2D GetTexture()
	{
		return requestCompleted ? texture : null;
	}

	private int test = 0;

	public IEnumerator Fetch()
	{
		if (requestPending)
		{
			requestPending = false;
			requestCompleted = false;

			texture = null;

			if (textureUrl != string.Empty)
			{
				if (textureUrl.StartsWith("http"))
				{
					using var unityWebRequest = UnityWebRequestTexture.GetTexture(textureUrl);
					yield return unityWebRequest.SendWebRequest();

					if (unityWebRequest.result != UnityWebRequest.Result.Success)
					{
						Debug.LogWarning($"{textureUrl}: {unityWebRequest.error}");
					}
					else
					{
						var downloadedTexture = DownloadHandlerTexture.GetContent(unityWebRequest);

						texture = new Texture2D(downloadedTexture.width, downloadedTexture.height,
							downloadedTexture.format, true);
						texture.LoadImage(unityWebRequest.downloadHandler.data);
						texture.Apply();

						Debug.Log($"Downloaded {textureUrl}");

#if SAVE_TEXTURE_TO_DISK
			            try
			            {
			                // Choose your destination folder
			                string folderPath = Path.Combine(Application.persistentDataPath, "DownloadedTextures");

			                // Make sure the folder exists
			                if (!Directory.Exists(folderPath))
			                    Directory.CreateDirectory(folderPath);

			                // Create a safe filename from the URL
			                string fileName = Path.GetFileNameWithoutExtension(textureUrl);
			                if (string.IsNullOrWhiteSpace(fileName))
			                    fileName = $"texture{test++}";

			                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
			                string fullPath = Path.Combine(folderPath, $"{fileName}.png");

			                // Encode and write to disk
			                byte[] pngData = texture.EncodeToPNG();
			                File.WriteAllBytes(fullPath, pngData);

			                Debug.Log($"✅ Saved texture to: {fullPath}");
			            }
			            catch (Exception e)
			            {
			                Debug.LogError($"❌ Failed to save texture: {e.Message}");
			            }
#endif

						requestCompleted = true;
					}
				}

				else
				{
					if (File.Exists(textureUrl))
					{
						var bytes = File.ReadAllBytes(textureUrl);

						texture = new Texture2D(1, 1);

						texture.LoadImage(bytes);

						texture.Apply();

						requestCompleted = true;
					}
				}
			}
		}
	}
}
