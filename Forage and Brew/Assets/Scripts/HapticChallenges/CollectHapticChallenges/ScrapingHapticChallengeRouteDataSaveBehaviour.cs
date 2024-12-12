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

#if UNITY_EDITOR
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
        
        EditorUtility.SetDirty(scrapingHapticChallengeSo);
    }
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < splineContainer.Spline.Count; i++)
        {
            if (splineContainer.Spline.Knots.ToArray()[i].Position.x < -3f)
            {
                splineContainer.Spline.SetKnot(i, new BezierKnot(new Vector3(-3f, splineContainer.Spline.Knots.ToArray()[i].Position.y, 0f)));
            }
            else if (splineContainer.Spline.Knots.ToArray()[i].Position.x > 3f)
            {
                splineContainer.Spline.SetKnot(i, new BezierKnot(new Vector3(3f, splineContainer.Spline.Knots.ToArray()[i].Position.y, 0f)));
            }

            if (splineContainer.Spline.Knots.ToArray()[i].Position.y < -1f)
            {
                splineContainer.Spline.SetKnot(i, new BezierKnot(new Vector3(splineContainer.Spline.Knots.ToArray()[i].Position.x, -1f, 0f)));
            }
            else if (splineContainer.Spline.Knots.ToArray()[i].Position.y > 1f)
            {
                splineContainer.Spline.SetKnot(i, new BezierKnot(new Vector3(splineContainer.Spline.Knots.ToArray()[i].Position.x, 1f, 0f)));
            }
            
            splineContainer.Spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
    }
#endif
}
