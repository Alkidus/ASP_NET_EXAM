using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    [Flags]
    public enum GenreType : int
    {
        Fantasy = 1,
        Mythology = 2,
        Adventure = 4,
        Tragedy = 8,
        Mystery = 16,
        Drama = 32,
        Romance = 64,
        Horror = 128,
        Satire = 256,
        Action = 512
    }
}
