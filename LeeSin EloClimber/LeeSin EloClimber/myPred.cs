using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

namespace LeeSin_EloClimber
{
    internal class resultPred
    {
        public Vector3 predPos;
        public int Hitchance;

        // Default constructor:
        public resultPred()
        {
        }

        // Constructor:
        public resultPred(Vector3 pos, int hit)
        {
            this.predPos = pos;
            this.Hitchance = hit;
        }
    }

    internal class myPred
    {
        internal static resultPred GetPrediction(Obj_AI_Hero target, Spell isSpell)
        {
            resultPred result = new resultPred(new Vector3(), 0);

            if (target != null & target.IsValidTarget(LeeSin.Q.Range + LeeSin.W.Range))
            {
                float timeToHit = (LeeSin.myHero.Position.Distance(target.Position) / isSpell.Speed) + (isSpell.Delay / 1000) + ((Game.Ping/1000)/2);
                float DistanceRun = target.MoveSpeed * timeToHit;

                Vector3[] path = target.Path;
                if (path.Count() == 1)
                {
                    if (path[0].Distance(LeeSin.myHero.Position) < isSpell.Range)
                        result.Hitchance = 8;

                    result.predPos = path[0];
                }
                else if (path.Count() == 2)
                {
                    Vector3 pos = path[1];
                    pos = target.Position + (pos - target.Position).Normalized() * (DistanceRun-(target.BoundingRadius/2)-(isSpell.Width/2));
                    if (target.Position.Distance(path[1]) < target.Position.Distance(pos))
                        pos = path[1];

                    if (pos.Distance(LeeSin.myHero.Position) < isSpell.Range)
                    {
                        if (DistanceRun > 500)
                            result.Hitchance = 3;
                        else if (DistanceRun > 400)
                            result.Hitchance = 4;
                        else if (DistanceRun > 300)
                            result.Hitchance = 5;
                        else if (DistanceRun > 200)
                            result.Hitchance = 6;

                        result.predPos = pos;
                    }

                }

                PredictionInput predInput = new PredictionInput { From = LeeSin.myHero.Position, Radius = isSpell.Width, Range = isSpell.Range };
                predInput.CollisionObjects[0] = CollisionableObjects.Heroes;
                predInput.CollisionObjects[1] = CollisionableObjects.Minions;

                IEnumerable<Obj_AI_Base> rCol = Collision.GetCollision(new List<Vector3> {  result.predPos }, predInput).ToArray();
                IEnumerable<Obj_AI_Base> rObjCol = rCol as Obj_AI_Base[] ?? rCol.ToArray();

                if (rObjCol.Count() > 0)
                    result.Hitchance = 0;
            }
            return result;
        }
    }
}
