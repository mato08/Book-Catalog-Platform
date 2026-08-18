using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Application.Features.Books
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Biography { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}
