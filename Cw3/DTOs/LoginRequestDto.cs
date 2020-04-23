using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Haslo { get; set; }
    }
}
