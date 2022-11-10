using System;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;

namespace Veronenger.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {
        // [Inject] private GameManager GameManager;
        [Inject] private CharacterManager CharacterManager { get; set; }

        public bool IsPlayer(CharacterBody2D player) {
            return CharacterManager.IsPlayer(player);
        }

        
    }
}