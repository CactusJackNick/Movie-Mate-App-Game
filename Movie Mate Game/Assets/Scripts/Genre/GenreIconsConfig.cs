using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Genre
{
    [CreateAssetMenu(menuName = "GenreConfigs", fileName = "GenreIconsConfig")]
    public class GenreIconsConfig :  ScriptableObject
    {
        [SerializeField] private List<GenreIconMapping> _icons = new();

        public Sprite GetIconFromId(int id)
        {
            foreach (var icon in _icons)
            {
                if (icon.Id == id)
                {
                    return icon.Icon;
                }
            }
            return null;
        }
    }
}