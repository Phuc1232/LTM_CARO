using caro.share.DTOs.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class LoginRequestDTO
    {
        public string username { get; set; }
    }
    public class LoginResponseDTO
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
    }
}
