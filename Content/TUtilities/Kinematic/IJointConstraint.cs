using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrapain.Content.TUtilities.Kinematic
{
    public interface IJointConstraint
    {
        public double ApplyPenaltyLoss(Joint owner, float gradientDescentCompletion);
    }
}
