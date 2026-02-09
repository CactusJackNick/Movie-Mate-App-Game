using System.Collections.Generic;
using DefaultNamespace.Models;
using UnityEngine;

namespace DefaultNamespace.Game
{
    public interface IClueFactory
    {
        List<ClueData> AssignDataToClues(DetailsSuperlistModel movie, Sprite poster, Sprite backdrop);
    }
}