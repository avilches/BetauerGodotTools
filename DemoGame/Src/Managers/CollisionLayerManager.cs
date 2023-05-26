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
        playerNode.CharacterBody2D.EnableAllShapes();

        playerNode.PlayerDetector.CollisionLayer = 0;
        playerNode.PlayerDetector.CollisionMask = 0;
        playerNode.PlayerDetector.Monitoring = true;
        playerNode.PlayerDetector.Monitorable = true;
        playerNode.PlayerDetector.AddToLayer(LayerPlayerDetectorArea);
        playerNode.PlayerDetector.DetectLayer(LayerPickableArea);
        playerNode.PlayerDetector.DetectLayer(LayerStageArea);
        playerNode.PlayerDetector.EnableAllShapes();
        
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

    public static void PlayerPickableItem(PickableItemNode pickableItemNode) {
        pickableItemNode.CharacterBody2D.CollisionLayer = 0;
        pickableItemNode.CharacterBody2D.CollisionMask = 0;
        pickableItemNode.CharacterBody2D.DetectLayer(LayerSolidBody);

        pickableItemNode.PickZone.Monitoring = true;
        pickableItemNode.PickZone.Monitorable = true;
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

    public static void PlayerConfigureAttackArea(Area2D attackArea) {
        attackArea.CollisionMask = 0;
        attackArea.CollisionLayer = 0;
        attackArea.Monitorable = true;
        attackArea.Monitoring = true;
        attackArea.EnableAllShapes();
        attackArea.DetectLayer(LayerEnemyHurtArea);
    }

    public static void PlayerConfigureHurtArea(Area2D hurtArea) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        hurtArea.Monitorable = true;
        hurtArea.Monitoring = true;
        hurtArea.EnableAllShapes();
        hurtArea.DetectLayer(LayerPlayerHurtArea);
    }

    public static void PlayerConfigureBulletRaycast(PhysicsRayQueryParameters2D rayCast2D) {
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
        enemy.EnableAllShapes();
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
        attackArea.Monitoring = true;
        attackArea.Monitorable = true;
        attackArea.AddToLayer(LayerPlayerHurtArea);
        attackArea.GetNode<CollisionShape2D>("Body").Disabled = false;
        attackArea.GetNode<CollisionShape2D>("Weapon").Disabled = true;
    }

    public static void EnemyConfigureHurtArea(Area2D hurtArea) {
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        hurtArea.Monitoring = true;
        hurtArea.Monitorable = true;
        hurtArea.AddToLayer(LayerEnemyHurtArea);
        hurtArea.EnableAllShapes();
    }

    public static void DisableAttackAndHurtAreas(Area2D attackArea, Area2D hurtArea) {
        // Raycasts (bullets) ignore Monitorable, so to disable the hurt area, it's better to remove the mask/layer 
        hurtArea.CollisionMask = 0;
        hurtArea.CollisionLayer = 0;
        attackArea.Monitoring = true;
        attackArea.Monitorable = true;
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

    public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);
}