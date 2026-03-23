using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentSelectorArturW.Models
{
    internal class Student
    {
        public int Number { get; set; } = 0;
        public string Name { get; set; } = string.Empty;

        public override string ToString() => $"{Number}. {Name}";
    }
}

