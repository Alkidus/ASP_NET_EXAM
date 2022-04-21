using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public int Value { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
    }
}
