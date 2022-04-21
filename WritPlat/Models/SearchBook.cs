using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    public class SearchBook
    {
        public string AuthorName { get; set; }
        public string BookName { get; set; }
        public List<EnumModel> Genres { get; set; }
        public bool IsTopByGenre { get; set; }

        public List<Book> Result { get; set; }
    }
}
