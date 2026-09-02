using Terraria;
using Terraria.ID;
using AlgoLib.Algo.Path;
using Terrapain.Content.TUtilities;

namespace Terrapain.Content.NPCs.VanillaNPCs
{
    public class Harpy : NPCBehaviour
    {
        public override int type => NPCID.Harpy;
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool ModPreAI(NPC npc)
        {
            if (CheckTarget(npc))
            {
                npc.TargetClosest();
            }
            else
            {
                bool IsWakeable(Point point)
                {
                    for (int x = 0; x < npc.width >> 4; x++)
                    {
                        for (int y = 0; y < npc.height >> 4; y++)
                        {
                            int hui = npc.height >> 4;
                            if (Main.tile[point.X + x, point.Y + y].IsSolid())
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                var Points = npc.FindPath(t.Target.Center.ToTileCoordinates(), 50);//AStarPathfinder.FindPath(npc.Center.ToTileCoordinates(), t.Target.Center.ToTileCoordinates(), IsWakeable, 500);
                //if (Points != null)
                //{
                //    foreach(var point in Points)
                //    {
                //        Dust.NewDust(point, 0, 0, DustID.BlueTorch);
                //    }
                //}
            }
            return false;
        }

        bool CheckTarget(NPC npc)
        {
            if (npc.target < 0)
            {
                return true;
            }
            if (!t.Target.active || t.Target.dead || t.Target.Distance(npc.Center) > 2500)
            {
                return true;
            }
            return false;
        }
        float right;
        float left;
        float up;
        float down;
        void CheckTilesAround(NPC npc)
        {
            down = Functions.RayColisionInTheWorld(npc.Center, Vector2.UnitY * 100 + npc.Center).Y - npc.Center.Y;
            if (down == -npc.Center.Y)
            {
                down = 100;
            }
            up = npc.Center.Y - Functions.RayColisionInTheWorld(npc.Center, Vector2.UnitY * -100 + npc.Center).Y;
            if (up == npc.Center.Y)
            {
                up = 100;
            }
            right = Functions.RayColisionInTheWorld(npc.Center, Vector2.UnitY * 100 + npc.Center).X - npc.Center.X;
            if (right == -npc.Center.Y)
            {
                right = 100;
            }
            left = npc.Center.X - Functions.RayColisionInTheWorld(npc.Center, Vector2.UnitY * -100 + npc.Center).X;
            if (left == npc.Center.X)
            {
                left = 100;
            }
        }
    }
}