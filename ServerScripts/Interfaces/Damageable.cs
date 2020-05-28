using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    interface Damageable
    {
        bool TakeDamage(float amount,int id);
    }
}
