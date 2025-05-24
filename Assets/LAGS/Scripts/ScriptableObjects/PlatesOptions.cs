using LAGS.Pub;
using UnityEngine;

namespace LAGS.ScriptablesObjects.Plates
{
    [CreateAssetMenu(fileName = "Plates", menuName = "LAGS/Plates", order = 1)]
    public class PlatesOptions : ScriptableObject
    {
        public Plate[] Plates;
        
        public Plate GetPlate(string id)
        {
            foreach (var plate in Plates)
            {
                if (plate.Id == id)
                {
                    return plate;
                }
            }

            Debug.LogError($"Plate with id {id} not found");
            return default;
        }

        public Plate GetRandomPlate()
        {
            return Plates[Random.Range(0, Plates.Length)];
        }
    }
}
