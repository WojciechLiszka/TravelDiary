using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Infrastructure.Repositories
{
    internal class EntryRepository : IEntryRepository
    {
        public Task Create(Entry entry)
        {
            throw new NotImplementedException();
        }
    }
}
