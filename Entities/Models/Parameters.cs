﻿using Dapper;

namespace Entities.Models
{
    public class Parameters
    {
        public DynamicParameters dynamicParameters { get; set; }
        public List<arg> arguments { get; set; }
        public string preparedQuery { get; set; }
    }
    public class arg
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
