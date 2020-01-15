using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperLibrary.Models
{
    public class FullPersonModel : PersonModel
    {
        public int Id { get; set; }
        public PhoneModel CellPhone { get; set; }
    }
}
