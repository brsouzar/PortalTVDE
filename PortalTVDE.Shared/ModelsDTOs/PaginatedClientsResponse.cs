using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class PaginatedClientsResponse
    {
        public List<ClientDtoWithId> Items { get; set; } = new List<ClientDtoWithId>();
        public int TotalCount { get; set; }
    }

    public class ClientDtoWithId : ClientDto
    {
        public int Id { get; set; }
    }
}
