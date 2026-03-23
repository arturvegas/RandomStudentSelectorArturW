using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomStudentSelectorArturW.Models;

namespace RandomStudentSelectorArturW.Services
{
    internal class DrawService
    {
        private static readonly Random rnd = new();

        public Student Draw(List<Student> students)
        {
            if (students == null || students.Count == 0)
                return null;

            var rnd = new Random();
            return students[rnd.Next(students.Count)];
        }
    }
}
