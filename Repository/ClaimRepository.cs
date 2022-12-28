using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Repository
{
    public class ClaimRepository : RepositoryBase<Claim>, IClaimRepository
    {
        public ClaimRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public void DeleteClaim(Claim Claim) => Delete(Claim);
    }
}
