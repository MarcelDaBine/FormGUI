using System.Collections.Generic;

namespace SquadGUI.ViewModels.Models;

public class BallisticTest {
    public TestParameters TestParameters { get; set; } = new();
    public List<Shot> Shots { get; set; } = [];
    public V50Result Result { get; set; } = new();

    public void AddShot(Shot shot) {
        Shots.Add(shot);
    }

    public bool RemoveShot(Shot shot) {
        return Shots.Remove(shot);
    }

    public void ClearShots() {
        Shots.Clear();
    }
}