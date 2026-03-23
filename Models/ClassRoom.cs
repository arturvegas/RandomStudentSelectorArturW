using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentSelectorArturW.Models
{
    internal class ClassRoom
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}