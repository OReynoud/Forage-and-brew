using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class GrindingHapticChallengeRouteDataSaveBehaviour : MonoBehaviour
{
    [SerializeField] private GrindingHapticChallengeSo grindingHapticChallengeSo;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Object grindingHapticChallengeRouteDataFolder;

#if UNITY_EDITOR
    [Button]
    private void SaveRouteData()
    {
        if (grindingHapticChallengeSo == null)
        {
            Debug.LogError("GrindingHapticChallengeSo is null.");
            return;
        }

        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is null.");
            return;
            
        }

        GrindingHapticChallengeRouteSo grindingHapticChallengeRouteSo = ScriptableObject.CreateInstance<GrindingHapticChallengeRouteSo>();
        
        foreach (BezierKnot knot in splineContainer.Spline.Knots)
        {
            grindingHapticChallengeRouteSo.Points.Add(knot.Position);
        }

        grindingHapticChallengeSo.Routes.Add(grindingHapticChallengeRouteSo);

        string[] currentFiles = AssetDatabase.FindAssets("t:ScriptableObject", new[] {AssetDatabase.GetAssetPath(grindingHapticChallengeRouteDataFolder)})
            .Select(AssetDatabase.GUIDToAssetPath).ToArray();
        
        int i = 1;
        while (currentFiles.Contains(AssetDatabase.GetAssetPath(grindingHapticChallengeRouteDataFolder) +
            $"/D_GrindingHapticChallengeRoute{i}.asset"))
        {
            i++;
        }
        
        AssetDatabase.CreateAsset(grindingHapticChallengeRouteSo,
            AssetDatabase.GetAssetPath(grindingHapticChallengeRouteDataFolder) +
            $"/D_GrindingHapticChallengeRoute{i}.asset");
        
        EditorUtility.SetDirty(grindingHapticChallengeSo);
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
