using Betauer.Core.Nodes;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Worlds;
using static Veronenger.LayerConstants;

namespace Veronenger.Managers; 

public class CollisionLayerManager {
    private const string GROUP_ENEMY = "enemy";
    public static void PlayerConfigureCollisions(PlayerNode playerNode) {
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

    public static void PlayerPickableArea(PlayerNode playerNode, Area2D.AreaEnteredEventHandler onEnterPickableArea) {
        playerNode.PlayerDetector.OnAreaEntered(LayerPickableArea, onEnterPickableArea, false, true);
    }

    public static void PickableItem(PickableItemNode pickableItemNode) {
        pickableItemNode.CharacterBody2D.CollisionLayer = 0;
        pickableItemNode.CharacterBody2D.CollisionMask = 0;
        pickableItemNode.CharacterBody2D.DetectLayer(LayerSolidBody);

        pickableItemNode.PickZone.CollisionLayer = 0;
        pickableItemNode.PickZone.CollisionMask = 0;
        pickableItemNode.PickZone.AddToLayer(LayerPickableArea);
    }

    public static void PlayerConfigureRaycastDrop(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = false;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.HitFromInside = true;
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public static void PlayerConfigureAttackArea(Area2D attackArea, Area2D.AreaEnteredEventHandler? onAttack = null) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        if (onAttack != null) attackArea.OnAreaEntered(LayerEnemyHurtArea, onAttack);
        else attackArea.DetectLayer(LayerEnemyHurtArea);
    }

    public static void PlayerConfigureHurtArea(Area2D hurtArea, Area2D.AreaEnteredEventHandler? onAttack = null) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        if (onAttack != null) hurtArea.OnAreaEntered(LayerPlayerHurtArea, onAttack);
        else hurtArea.DetectLayer(LayerPlayerHurtArea);
    }

    public static void PlayerConfigureBullet(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.HitFromInside = true;
        rayCast2D.DetectLayer(LayerEnemyHurtArea);
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public static void NpcConfigureCollisions(CharacterBody2D enemy) {
        enemy.AddToGroup(GROUP_ENEMY);
        enemy.CollisionMask = 0;
        enemy.CollisionLayer = 0;                                       
        enemy.DetectLayer(LayerSolidBody);
    }

    public static void NpcConfigureCollisions(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = false;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public static void NpcConfigureCollisions(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = false;
        rayCast2D.CollideWithBodies = true;
        rayCast2D.DetectLayer(LayerSolidBody);
    }

    public static void EnemyConfigureAttackArea(Area2D attackArea) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        attackArea.AddToLayer(LayerPlayerHurtArea);
    }

    public static void EnemyConfigurePlayerDetector(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = false;
        rayCast2D.DetectLayer(LayerPlayerDetectorArea);
    }

    public static void EnemyConfigurePlayerDetector(PhysicsRayQueryParameters2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.CollideWithAreas = true;
        rayCast2D.CollideWithBodies = false;
        rayCast2D.DetectLayer(LayerPlayerDetectorArea);
    }

    public static void EnemyConfigureHurtArea(Area2D hurtArea) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        hurtArea.AddToLayer(LayerEnemyHurtArea);
    }

    public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

}