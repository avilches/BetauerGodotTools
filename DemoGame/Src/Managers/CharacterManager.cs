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
    [Inject] private EventBus EventBus { get; set; }

    public void RegisterPlayerNode(PlayerNode playerNode) {
        PlayerNode = playerNode;
    }

    public void PlayerConfigureCollisions(PlayerNode playerNode) {
        playerNode.CharacterBody2D.CollisionLayer = 0;
        playerNode.CharacterBody2D.CollisionMask = 0;
        playerNode.CharacterBody2D.AddToLayer(LayerPlayerBody);
        playerNode.CharacterBody2D.DetectLayer(LayerSolidBody);

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

    public void PlayerConfigureAttackArea(Area2D attackArea, Area2D.AreaEnteredEventHandler? onAttack = null) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        if (onAttack != null) attackArea.OnAreaEntered(LayerEnemyHurtArea, onAttack);
        else attackArea.DetectLayer(LayerEnemyHurtArea);
    }

    public void PlayerConfigureHurtArea(Area2D hurtArea, Area2D.AreaEnteredEventHandler? onAttack = null) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        if (onAttack != null) hurtArea.OnAreaEntered(LayerPlayerHurtArea, onAttack);
        else hurtArea.DetectLayer(LayerPlayerHurtArea);
    }

    public void PlayerConfigureBullet(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.HitFromInside = true;
        rayCast2D.DetectLayer(LayerEnemyHurtArea);
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public void NpcConfigureCollisions(CharacterBody2D enemy) {
        enemy.AddToGroup(GROUP_ENEMY);
        enemy.CollisionMask = 0;
        enemy.CollisionLayer = 0;                                       
        enemy.DetectLayer(LayerSolidBody);
    }

    public void NpcConfigureCollisions(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = false;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public void NpcConfigureCollisions(PhysicsRayQueryParameters2D rayCast2D) {
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

    public void EnemyConfigurePlayerDetector(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = false;
        rayCast2D.DetectLayer(LayerPlayerDetectorArea);
    }

    public void EnemyConfigureHurtArea(Area2D hurtArea) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        hurtArea.AddToLayer(LayerEnemyHurtArea);
    }

    public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

    public bool IsPlayer(CharacterBody2D player) {
        return PlayerNode.CharacterBody2D == player;
    }

}