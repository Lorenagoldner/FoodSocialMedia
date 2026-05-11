using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppFood.Data
{
    public class SqlConnectionConfiguration
    {
        public string _value { get; }
        public SqlConnectionConfiguration(string value)
        {
            _value = value;
        }
    }
}
