using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class ScrapingHapticChallengeRouteDataSaveBehaviour : MonoBehaviour
{
    [SerializeField] private ScrapingHapticChallengeSo scrapingHapticChallengeSo;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Object scrapingHapticChallengeRouteDataFolder;

    [Button]
    private void SaveRouteData()
    {
        if (scrapingHapticChallengeSo == null)
        {
            Debug.LogError("ScrapingHapticChallengeSo is null.");
            return;
        }

        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is null.");
            return;
            
        }

        ScrapingHapticChallengeRouteSo scrapingHapticChallengeRouteSo = ScriptableObject.CreateInstance<ScrapingHapticChallengeRouteSo>();
        
        foreach (BezierKnot knot in splineContainer.Spline.Knots)
        {
            scrapingHapticChallengeRouteSo.Points.Add(knot.Position);
        }

        scrapingHapticChallengeSo.Routes.Add(scrapingHapticChallengeRouteSo);

        string[] currentFiles = AssetDatabase.FindAssets("t:ScriptableObject", new[] {AssetDatabase.GetAssetPath(scrapingHapticChallengeRouteDataFolder)})
            .Select(AssetDatabase.GUIDToAssetPath).ToArray();
        
        int i = 1;
        while (currentFiles.Contains(AssetDatabase.GetAssetPath(scrapingHapticChallengeRouteDataFolder) +
            $"/D_ScrapingHapticChallengeRoute{i}.asset"))
        {
            i++;
        }
        
        AssetDatabase.CreateAsset(scrapingHapticChallengeRouteSo,
            AssetDatabase.GetAssetPath(scrapingHapticChallengeRouteDataFolder) +
            $"/D_ScrapingHapticChallengeRoute{i}.asset");
    }
}
