using System;
using Betauer.Core.Nodes;
using Godot;
using Betauer.DI;
using Veronenger.Character.Enemy;
using Veronenger.Character.Player;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers; 

[Service]
public class CharacterManager {
    private const string GROUP_ENEMY = "enemy";

    public PlayerNode PlayerNode { get; private set; }
    [Inject] private Game Game { get; set; }

    [Inject] public PlatformManager PlatformManager { get; set;}
    [Inject] private Bus Bus { get; set; }

    public void RegisterPlayerNode(PlayerNode playerNode) {
        PlayerNode = playerNode;
    }

    public void PlayerConfigureCollisions(PlayerNode playerNode) {
        playerNode.PlatformBody.CharacterBody.CollisionLayer = 0;
        playerNode.PlatformBody.CharacterBody.CollisionMask = 0;
        playerNode.PlatformBody.CharacterBody.AddToLayer(LayerPlayerBody);
        playerNode.PlatformBody.CharacterBody.DetectLayer(LayerSolidBody);

        playerNode.PlayerDetector.CollisionLayer = 0;
        playerNode.PlayerDetector.CollisionMask = 0;
        playerNode.PlayerDetector.AddToLayer(LayerPlayerDetectorArea);
        
        playerNode.RaycastCanJump.DetectLayer(LayerSolidBody);
        playerNode.FloorRaycasts.ForEach(rayCast2D => {
            rayCast2D.CollideWithAreas = false;
            rayCast2D.CollideWithBodies = true;
            rayCast2D.DetectLayer(LayerSolidBody);
        });
    }

    public void PlayerConfigureAttackArea(Area2D attackArea) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        attackArea.AddToLayer(LayerEnemyHurtArea);
    }

    public void EnemyConfigureCollisions(CharacterBody2D enemy) {
        enemy.AddToGroup(GROUP_ENEMY);
        enemy.CollisionMask = 0;
        enemy.CollisionLayer = 0;                                       
        enemy.DetectLayer(LayerSolidBody);
    }

    public void PlayerConfigureHurtArea(Area2D damageArea, Action<Area2D> onAttack) {
        damageArea.CollisionMask = 0;
        damageArea.CollisionLayer = 0;
        damageArea.OnAreaEntered(LayerPlayerHurtArea, onAttack);
    }
    
    public void EnemyConfigureCollisions(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = false;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public void EnemyConfigureAttackArea(Area2D attackArea) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        attackArea.AddToLayer(LayerPlayerHurtArea);
    }

    public void EnemyConfigurePlayerDetector(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = false;
        rayCast2D.DetectLayer(LayerPlayerDetectorArea);
    }

    public void EnemyConfigureHurtArea(Area2D hurtArea, Action<Area2D> onAttack) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        hurtArea.OnAreaEntered(LayerEnemyHurtArea, onAttack);
    }

    public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

    public bool IsPlayer(CharacterBody2D player) {
        return PlayerNode.PlatformBody.CharacterBody == player;
    }

}