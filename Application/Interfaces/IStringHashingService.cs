using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IStringHashingService
    {
        public string CreateDESStringHash(string input);
        public string DecodeDESStringHash(string input);
    }
}
