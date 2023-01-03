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
        playerNode.CollisionLayer = 0;
        playerNode.CollisionMask = 0;
            
        playerNode.AddToLayer(LayerPlayerBody);
        playerNode.DetectLayer(LayerBodySolid);

        playerNode.RaycastCanJump.DetectLayer(LayerBodySolid);
        playerNode.FloorRaycasts.ForEach(rayCast2D => {
            rayCast2D.DetectLayer(LayerBodySolid);
        });
    }

    public void PlayerConfigureAttackArea2D(Area2D attackArea2D) {
        attackArea2D.CollisionMask = 0;
        attackArea2D.CollisionLayer = 0;
        attackArea2D.AddToLayer(LayerEnemyArea2D);
    }

    public void EnemyConfigureCollisions(CharacterBody2D enemy) {
        enemy.AddToGroup(GROUP_ENEMY);
        enemy.CollisionMask = 0;
        enemy.CollisionLayer = 0;                                       
        enemy.DetectLayer(LayerBodySolid);
    }

    public void EnemyConfigureCollisions(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.DetectLayer(LayerBodySolid);
    }

    public void EnemyConfigurePlayerDetector(RayCast2D rayCast2D) {
        rayCast2D.CollisionMask = 0;
        rayCast2D.DetectLayer(LayerPlayerBody);
    }

    public void EnemyConfigureDamageArea2D(Area2D enemyDamageArea2D, Action<Area2D> onAttack) {
        if (enemyDamageArea2D.GetParent() is not IEnemy) throw new Exception("Only enemies can use this method");
        enemyDamageArea2D.CollisionMask = 0;
        enemyDamageArea2D.CollisionLayer = 0;
        enemyDamageArea2D.OnAreaEntered(LayerEnemyArea2D, onAttack);
    }

    public bool IsEnemy(CharacterBody2D platform) => platform.IsInGroup(GROUP_ENEMY);

    public bool IsPlayer(CharacterBody2D player) {
        return PlayerNode == player;
    }

}