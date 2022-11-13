using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Tests.Signal {
    public class BaseNodeTest : Node {
        public Area2D CreateArea2D(string name = null, int x = 0, int y = 0) {
            var area2D = new Area2D();
            area2D.Position = new Vector2(x, y);
            area2D.Name = name;
            area2D.Monitoring = true;
            area2D.AddChild(CreateCollisionShape());
            
            AddChild(area2D);
            return area2D;
        }

        public CharacterBody2D CreateKinematicBody2D(string name, int x = 0, int y = 0) {
            var body2D = new CharacterBody2D();
            body2D.Position = new Vector2(x, y);
            body2D.Name = name;
            body2D.AddChild(CreateCollisionShape());
            
            AddChild(body2D);
            return body2D;
        }

        public CollisionShape2D CreateCollisionShape() {
            return new CollisionShape2D {
                Shape = new RectangleShape2D {
                    Size = new Vector2(4, 4)
                },
            };
        }
        
        
        public async Task ForceCollision(Area2D area2D, Node2D body) {
            await this.AwaitProcessFrame();
            body.Position = area2D.Position = Vector2.Zero;
            var x = 0;
            while (!CollideWith(area2D, body) && x < 60) {
                await this.AwaitPhysicsFrame();
                x++;
            }
            Console.WriteLine($"Waited {x} Physics Frames until collision");
        }

        private static bool CollideWith(Area2D area2D, Node2D body) {
            return body is Area2D otherArea2D ? 
                area2D.GetOverlappingAreas().Contains(otherArea2D) : 
                area2D.GetOverlappingBodies().Contains(body);
        }

        public async Task ForceNotCollision(Area2D area2D, Node2D body) {
            await this.AwaitProcessFrame();
            area2D.Position = new Vector2(-2000, -2000);
            body.Position = new Vector2(2000, 2000);
            var x = 0;
            while (CollideWith(area2D, body) && x < 60) {
                await this.AwaitPhysicsFrame();
                x++;
            }
            Console.WriteLine($"Waited {x} Physics Frames until collision");
        }

    }
}